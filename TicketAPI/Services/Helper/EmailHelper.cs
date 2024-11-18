using TicketAPI.Data.Models;

namespace TicketAPI.Services.Helper;

public class EmailHelper(LinkGenerator linkGenerator, IEmailSender emailSender)
{
    public void GenerateVerificationEmail(string code, HttpContext context, string email, string userId)
    {
        var callbackUrl = linkGenerator.GetUriByAction(
            context,
            Constants.Constants.ConfirmEmailController,
            "Email",
            new { userId = userId, code = code},
            context.Request.Scheme);
         emailSender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>\"");
    }

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
        emailSender.SendEmailAsync(
            email,
            "Reset Password",
            $"Please reset your password by clicking this link: <a href='{callbackUrl}'>link</a>");
    }
}