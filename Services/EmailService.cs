using Booking.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace Booking.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "your-email@gmail.com";
                var fromPassword = _configuration["EmailSettings:FromPassword"] ?? "your-app-password";

                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail, "Booking System");
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                client.EnableSsl = true;

                await client.SendMailAsync(message);

                _logger.LogInformation("[Email Service]: Đã gửi thành công email tới {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Email Service]: Gửi email tới {Email} thất bại!", toEmail);
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string fullName, string otpCode)
        {
            string subject = "Mã xác thực tài khoản Booking";

            string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee;'>
                    <h3>Xin chào {fullName},</h3>

                    <p>Tài khoản của bạn đã được tạo thành công.</p>

                    <p>Vui lòng nhập mã OTP dưới đây để kích hoạt tài khoản:</p>

                    <div style='text-align:center;
                                font-size:32px;
                                font-weight:bold;
                                letter-spacing:8px;
                                color:#28a745;
                                margin:25px 0;'>
                        {otpCode}
                    </div>

                    <p>Mã OTP có hiệu lực trong <strong>5 phút</strong>.</p>

                    <p>Nếu bạn không yêu cầu tạo tài khoản, vui lòng bỏ qua email này.</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
