using Booking.DTOs.User.Request;
using Booking.DTOs.User.Response;
using Booking.Entities.User.User;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Mappings;

namespace Booking.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserResponseDTO> CreateUserByAdminAsync(CreateUserDTO request)
        {
            try
            {
                bool isEmailExists = await _userRepository.ExistsByEmail(request.Email);
                if (isEmailExists)
                {
                    throw new Exception($"Email '{request.Email}' đã được sử dụng trên hệ thống.");
                }

                var user = new User
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role,
                    Status = UserStatus.Pending
                };

                string hashedPassword = $"[Hashed]_{request.Password}";

                user.ChangePassword(hashedPassword);

                user.Status = UserStatus.Pending;

                var createdUser = await _userRepository.Create(user);

                Console.WriteLine($"[Email System]: Đã gửi link kích hoạt đến email {createdUser.Email}");

                return UserMapper.ToResponseDTO(createdUser);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error - CreateUserByAdminAsync]: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAll();
                return users.Select(user => UserMapper.ToResponseDTO(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error - GetAllUsersAsync]: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new ArgumentException("Email tìm kiếm không được để trống.");
                }
                return await _userRepository.GetByEmail(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error - GetUserByEmailAsync]: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                }
                return await _userRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error - GetUserByIdAsync]: {ex.Message}");
                throw;
            }
        }

        public async Task RestoreUserAsync(int id)
        {
            try
            {
                await _userRepository.Restore(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error - RestoreUserAsync]: {ex.Message}");
                throw;
            }
        }

        public async Task SoftDeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetById(id);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng cần xóa hoặc tài khoản đã bị xóa trước đó.");
                }

                user.SoftDelete();

                await _userRepository.SoftDelete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error - SoftDeleteUserAsync]: {ex.Message}");
                throw;
            }
        }

        public Task VerifyEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

    }
}
