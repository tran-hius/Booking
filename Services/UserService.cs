using Booking.DTOs.User.Request;
using Booking.DTOs.User.Response;
using Booking.Entities.User.User;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Mappings;
using Booking.Exceptions;
using Booking.Entities.User;

namespace Booking.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
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

                _logger.LogInformation("[Email System]: Đã gửi link kích hoạt đến email {Email}", createdUser.Email);

                return UserMapper.ToResponseDTO(createdUser);
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

        public Task VerifyEmailAsync(string email)
        {
            throw new NotImplementedException();
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
