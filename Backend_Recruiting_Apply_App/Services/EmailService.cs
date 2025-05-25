namespace Backend_Recruiting_Apply_App.Services
{
    using Backend_Recruiting_Apply_App.Data.Entities;
    using MailKit.Net.Smtp;
    using MimeKit;
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest request);
    }

    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _defaultSenderEmail = "crisshoang0711@gmail.com"; // Email mặc định
        private readonly string _senderPassword = "klrj zrap itiv avtu";        // Mật khẩu hoặc App Password

        public async Task SendEmailAsync(EmailRequest request)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Compose))
            {
                throw new ArgumentException("Các trường To, Subject và Compose là bắt buộc.");
            }

            // Tạo đối tượng email
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Thông báo", _defaultSenderEmail)); // Luôn dùng _defaultSenderEmail
            email.To.Add(new MailboxAddress("", request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart("plain") { Text = request.Compose };

            // Kết nối và gửi email
            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_defaultSenderEmail, _senderPassword);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi gửi email: {ex.Message}");
                }
            }
        }
    }
}
