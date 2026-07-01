using System.ComponentModel.DataAnnotations;

namespace Booking.DTOs.User.Request
{
    public class VerifyEmailDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; } = string.Empty;
    }
}