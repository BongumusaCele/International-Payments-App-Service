using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace InternationalPaymentsAPI.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendOtpAsync(string toAddress, string otpCode)
        {
            if (!_options.EnableSending)
            {
                _logger.LogWarning("Email sending is disabled. MFA OTP for {Email} is {OtpCode}", toAddress, otpCode);
                return;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.FromAddress, _options.FromName),
                Subject = "Your International Payments verification code",
                Body = $"Your verification code is {otpCode}. It expires in 10 minutes.",
                IsBodyHtml = false
            };
            message.To.Add(toAddress);

            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            await client.SendMailAsync(message);
        }
    }
}
