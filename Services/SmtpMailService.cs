using DataAccess;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Services
{
    public class SmtpMailService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        public SmtpMailService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task SendMailAsync(string recipientEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(
                _configuration["SmtpSettings:SenderName"] ?? "",
                _configuration["SmtpSettings:SenderEmail"] ?? ""));

            emailMessage.To.Add(new MailboxAddress("", recipientEmail));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder();
            string appName = _dbContext.SystemInfo.FirstOrDefault()?.AppName ?? "Hệ thống LifeSoft";
            builder.HtmlBody = $@"
                <html>
                    <body>
                        <h1 style='color: #333333;'>{appName.Replace("<br />", " ")}</h1>
                        <p>Xin chào!!!</p>
                        <p>{message}.</p>
                        <p>---</p>
                        <p>Thư trên được gửi tự động từ hệ thống. Vui lòng không reply lại. Xin cảm ơn!</p>
                    </body>
                </html>";
            emailMessage.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                // Bỏ qua xác thực chứng chỉ trong môi trường phát triển
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                await client.ConnectAsync(
                    _configuration["SmtpSettings:Server"] ?? "",
                    int.Parse(_configuration["SmtpSettings:Port"] ?? "587"),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["SmtpSettings:Username"] ?? "",
                    _configuration["SmtpSettings:Password"] ?? "");

                await client.SendAsync(emailMessage);
                Console.WriteLine("✅ Email đã được gửi thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi gửi email: {ex.Message}");
            }
        }
    }
}
