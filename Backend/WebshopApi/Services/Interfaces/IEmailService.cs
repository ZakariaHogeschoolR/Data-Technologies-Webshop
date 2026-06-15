using System.Net;
using System.Net.Mail;

public interface IEmailservice
{
    Task SendPasswordResetEmail(string toEmail, string resetLink);
}
