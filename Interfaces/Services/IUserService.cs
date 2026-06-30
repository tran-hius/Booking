using Booking.DTOs.User.Request;
using Booking.DTOs.User.Response;

namespace Booking.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> CreateUserByAdminAsync(CreateUserDTO request);
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task VerifyEmailAsync(string email);
        Task SoftDeleteUserAsync(int id);
        Task RestoreUserAsync(int id);

    }
}
