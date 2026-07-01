using Booking.DTOs.User.Request;
using Booking.DTOs.User.Response;
using Booking.Entities.User.User;

namespace Booking.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> CreateUserByAdminAsync(CreateUserDTO request);
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task VerifyEmailAsync(string email);
        Task<UserResponseDTO> UpdateUserAsync(int id, UpdateUserDTO request);
        Task SoftDeleteUserAsync(int id);
        Task RestoreUserAsync(int id);

    }
}
