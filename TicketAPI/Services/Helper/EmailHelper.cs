using System.Web;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Services.Helper;

/// <summary>
/// Generates email texts.
/// </summary>
public class EmailHelper(LinkGenerator linkGenerator, IEmailSender emailSender, ILogger<EmailHelper> logger, IWebHostEnvironment environment, IConfiguration configuration)
{
    /// <summary>
    /// Generates the text for a verification email.
    /// </summary>
    /// <param name="code">The code used.</param>
    /// <param name="context">The HttpContext.</param>
    /// <param name="email">The email receiver.</param>
    /// <param name="userId">The target user of the verification.</param>
    public void GenerateVerificationEmail(string code, HttpContext context, string email, string userId, string firstName)
    {
        var baseUrl = configuration["FrontEnd:BaseUrl"];
        
        var callbackUrl = new UriBuilder(new Uri(baseUrl))
        {
            Path = configuration["FrontEnd:EmailConfirmEndpunkt"]
        };
        
        var query  = HttpUtility.ParseQueryString(string.Empty);
        query["userId"] = userId;
        query["code"] = code;
        callbackUrl.Query = query.ToString();
        
        var path = Path.Combine(environment.ContentRootPath, "EmailTemplates/verification-email.html");
        var htmlTemplate = File.ReadAllText(path);
        htmlTemplate = htmlTemplate.Replace("{firstName}", firstName);
        htmlTemplate = htmlTemplate.Replace("{callbackUrl}", callbackUrl.ToString());
         emailSender.SendEmailAsync(email, "Confirm your email",
             htmlTemplate);
    }

    /// <summary>
    /// Generates an email to reset the password.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="email"></param>
    /// <param name="userId"></param>
    public void GenerateResetEmail(string code, string email, string userId, string firstName)
    {
        var callbackUrl = new UriBuilder(new Uri(configuration["FrontEnd:BaseUrl"]))
           {
               Path = configuration["FrontEnd:EmailForgetPasswordEndpunkt"]
           };
           
        var query  = HttpUtility.ParseQueryString(string.Empty);
        query["userId"] = userId;
        query["token"] = code;
        callbackUrl.Query = query.ToString();
        
        var path = Path.Combine(environment.ContentRootPath, "EmailTemplates/forgot-password.html");
        var htmlTemplate = File.ReadAllText(path);
        htmlTemplate = htmlTemplate.Replace("{firstName}", firstName);
        htmlTemplate = htmlTemplate.Replace("{callbackUrl}", callbackUrl.ToString());
        emailSender.SendEmailAsync(
            email,
            "Reset Password",
            htmlTemplate);
    }
}