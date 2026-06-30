using Booking.Entities.User;
using Booking.Entities.User.User;

namespace Booking.DTOs.User.Request
{
    public class CreateUserDTO
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Role Role { get; set; }
        public UserStatus Status { get; set; }
    }
}
