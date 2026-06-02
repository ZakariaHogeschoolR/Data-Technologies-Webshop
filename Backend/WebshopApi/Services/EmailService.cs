using System.Net;
using System.Net.Mail;

namespace Service;

public class EmailService: IEmailservice
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmail(string toEmail, string resetLink)
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
        var senderEmail = _configuration["Email:SenderEmail"];
        var senderPassword = _configuration["Email:SenderPassword"];

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = "Password Reset Request",
            Body = $@"
                <h2>Password Reset</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>This link expires in 1 hour.</p>
                <p>If you did not request this, ignore this email.</p>
            ",
            IsBodyHtml = true
        };

        message.To.Add(toEmail);
        await client.SendMailAsync(message);
    }
}
