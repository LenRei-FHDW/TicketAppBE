using TicketAPI.Services.Scoped;

namespace TicketAPI.Services.Helper;

/// <summary>
/// Generates email texts.
/// </summary>
public class EmailHelper(LinkGenerator _linkGenerator, IEmailSender _emailSender, ILogger<EmailHelper> _logger)
{
    /// <summary>
    /// Generates the text for a verification email.
    /// </summary>
    /// <param name="code">The code used.</param>
    /// <param name="context">The HttpContext.</param>
    /// <param name="email">The email receiver.</param>
    /// <param name="userId">The target user of the verification.</param>
    public void GenerateVerificationEmail(string code, HttpContext context, string email, string userId)
    {
        var callbackUrl = _linkGenerator.GetUriByAction(
            context,
            Constants.Constants.ConfirmEmailController,
            "Email",
            new { userId = userId, code = code},
            context.Request.Scheme);
         _emailSender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>\"");
    }

    /// <summary>
    /// Generates an email to reset the password.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="email"></param>
    /// <param name="userId"></param>
    public void GenerateResetEmail(string code, string email,
        string userId)
    {
        /*var callbackUrl = _linkGenerator.GetUriByAddress(
            address: "",
            values:  new RouteValueDictionary( new { userId = userId, token = code }),
            scheme: "https",
            host: new HostString(Constants.Constants.FrontendUrl));
        Console.WriteLine(callbackUrl);*/
        var callbackUrl = Constants.Constants.FrontendUrl + Constants.Constants.ResetPasswordPath + "?userId=" + userId + "&token=" + code;
        _emailSender.SendEmailAsync(
            email,
            "Reset Password",
            $"Please reset your password by clicking this link: <a href='{callbackUrl}'>link</a>");
    }
}