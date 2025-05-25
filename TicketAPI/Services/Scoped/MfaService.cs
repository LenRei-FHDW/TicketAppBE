using Microsoft.AspNetCore.Identity;
using QRCoder.Core;
using TicketAPI.Data.Models;

namespace TicketAPI.Services.Scoped;

public interface IMfaService
{
    Task<string> GenerateNewAuthenticatorKeyAsync(ApplicationUser user);
    Task<byte[]> GenerateQrCodeAsync(string email, string key);
    Task<bool> EnableAuthenticatorAsync(ApplicationUser user, string code);
    Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string code);
}

public class MfaService : IMfaService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string Issuer = "TicketAPI";

    public MfaService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<string> GenerateNewAuthenticatorKeyAsync(ApplicationUser user)
    {
        await _userManager.ResetAuthenticatorKeyAsync(user);
        return await _userManager.GetAuthenticatorKeyAsync(user);
    }

    public Task<byte[]> GenerateQrCodeAsync(string email, string key)
    {
        var otpauth = $"otpauth://totp/{Issuer}:{email}?secret={key}&issuer={Issuer}";

        using var qrGen = new QRCodeGenerator();
        using var qrData = qrGen.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        return Task.FromResult(qrCode.GetGraphic(20));
    }

    public async Task<bool> EnableAuthenticatorAsync(ApplicationUser user, string code)
    {
        var result = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            code);
        
        if (!result)
            return false;
        
        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return true;
    }

    public Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string code)
    {
        return _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
    }
}