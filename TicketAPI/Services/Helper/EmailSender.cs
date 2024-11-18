using System.Net;
using System.Net.Mail;

namespace TicketAPI.Services.Helper;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

/// <summary>
/// Used to send emails.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private SmtpClient client;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
        var emailSettings = _configuration.GetSection("EmailSettings");
        client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]));
        client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);
        client.EnableSsl = bool.Parse(emailSettings["EnableSsl"]);
    }

    /// <summary>
    /// Creates an email and sends it.
    /// </summary>
    /// <param name="email">The target of the mail.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="message">The content of the email.</param>
    /// <returns></returns>
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);
            
        await client.SendMailAsync(mailMessage);
    }
}