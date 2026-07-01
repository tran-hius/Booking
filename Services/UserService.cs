using Booking.DTOs.User.Request;
using Booking.DTOs.User.Response;
using Booking.Entities.User;
using Booking.Entities.User.User;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Mappings;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;

namespace Booking.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IDistributedCache _cache;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IEmailService emailService, IDistributedCache cache)
        {
            _userRepository = userRepository;
            _logger = logger;
            _emailService = emailService;
            _cache = cache;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string HashOtp(string otp)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
            return Convert.ToHexString(bytes);
        }
        public async Task<UserResponseDTO> CreateUserByAdminAsync(CreateUserDTO request)
        {

            bool isEmailExists = await _userRepository.ExistsByEmail(request.Email);
            if (isEmailExists)
            {
                throw new BusinessException($"Email '{request.Email}' đã được sử dụng trên hệ thống.");
            }

            string hashedPassword = HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Role = (Role)request.Role,
                PasswordHash = hashedPassword,
                Status = UserStatus.Pending
            };

            var createdUser = await _userRepository.Create(user);

            string otpCode = RandomNumberGenerator
                            .GetInt32(100000, 1000000)
                            .ToString();

            string hashedOtp = HashOtp(otpCode);

            string otpKey = $"otp:hash:{createdUser.Email}";
            string attemptKey = $"otp:attempts:{createdUser.Email}";


            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(otpKey, hashedOtp, cacheOptions);

            await _cache.SetStringAsync(attemptKey, "0", cacheOptions);

            

            await _emailService.SendOtpEmailAsync(createdUser.Email, createdUser.FullName, otpCode);

            _logger.LogInformation(
                        "[CreateUser] Đã tạo tài khoản {Email}",
                        createdUser.Email);

            return UserMapper.ToResponseDTO(createdUser);
        }

        public async Task VerifyEmailAsync(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            {
                throw new BusinessException("Thông tin xác thực không hợp lệ.");
            }
            string otpKey = $"otp:hash:{email}";
            string attemptKey = $"otp:attempts:{email}";

            string? hashedOtp = await _cache.GetStringAsync(otpKey);

            if (hashedOtp == null)
            {
                throw new BusinessException("Mã OTP đã hết hạn hoặc không tồn tại.");
            }

            string? attemptValue = await _cache.GetStringAsync(attemptKey);
            int attempts = int.TryParse(attemptValue, out int value) ? value : 0;

            if (attempts >= 5)
            {
                await _cache.RemoveAsync(otpKey);
                await _cache.RemoveAsync(attemptKey);

                throw new BusinessException("Bạn đã nhập sai OTP quá 5 lần. Vui lòng yêu cầu mã OTP mới.");
            }

            string inputHashedOtp = HashOtp(otp);


            bool isValid = string.Equals(
                inputHashedOtp,
                hashedOtp,
                StringComparison.Ordinal);

            if (!isValid)
            {
                attempts++;

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                await _cache.SetStringAsync(attemptKey, attempts.ToString(), cacheOptions);

                _logger.LogWarning(
                    "[VerifyEmail] OTP không hợp lệ cho {Email}. Lần thử: {Attempts}",
                    email,
                    attempts);

                throw new BusinessException("Mã OTP không chính xác.");
            }

            var user = await _userRepository.GetByEmail(email);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin thành viên.");
            }

            if (user.Status == UserStatus.Active)
            {
                throw new BusinessException("Tài khoản đã được kích hoạt.");
            }

            user.Status = UserStatus.Active;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.Update(user);

            await _cache.RemoveAsync(otpKey);
            await _cache.RemoveAsync(attemptKey);

            _logger.LogInformation("[VerifyEmail] Tài khoản {Email} đã được kích hoạt thành công.", email);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAll();
            return users.Select(user => UserMapper.ToResponseDTO(user));

        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new BusinessException("Email tìm kiếm không được để trống.");
            }
            return await _userRepository.GetByEmail(email);


        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BusinessException("ID người dùng không hợp lệ.");
            }
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng có ID = {id}");
            }
            return user;

        }

        public async Task RestoreUserAsync(int id)
        {
            await _userRepository.Restore(id);
        }

        public async Task SoftDeleteUserAsync(int id)
        {
            await _userRepository.SoftDelete(id);
        }

        public async Task<UserResponseDTO> UpdateUserAsync(int id, UpdateUserDTO request)
        {
            if (id <= 0)
            {
                throw new BusinessException("ID người dùng không hợp lệ.");
            }
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng có ID = {id}");
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email;

            if (request.Role.HasValue)
            {
                user.Role = request.Role.Value;
            }

            if (request.Status.HasValue)
                user.Status = request.Status.Value;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                string hashedPassword = HashPassword(request.Password);
                user.ChangePassword(hashedPassword);
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.Update(user);

            return UserMapper.ToResponseDTO(user);
        }
    }
}
