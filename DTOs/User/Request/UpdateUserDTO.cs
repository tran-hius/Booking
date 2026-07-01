using Booking.Entities.User;
using Booking.Entities.User.User;

namespace Booking.DTOs.User.Request
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public Role? Role { get; set; } 
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserStatus? Status { get; set; }
    }
}