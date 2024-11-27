using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;
using TicketAPI.Services.Scoped;
using TicketAPITests.Mocks;

namespace TicketAPITests.ServiceTests
{
    internal class PasswordServiceTests
    {
        private PasswordService passwordService;
        private ApplicationUser testUser;

        [SetUp]
        public void Setup()
        {
            testUser = new ApplicationUser { UserName = "test", Id = "123", Email = "test@email.com" };

            var store = new Mock<IUserStore<ApplicationUser>>();
            store.Setup(x => x.FindByIdAsync("123", CancellationToken.None))
                .ReturnsAsync(testUser);
            var userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(testUser);
            userManager.Setup(x => x.CheckPasswordAsync(testUser, "testPw")).ReturnsAsync(true);
            userManager.Setup(x => x.CreateAsync(testUser, "testPw")).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(testUser)).ReturnsAsync("testToken");
            userManager.Setup(x => x.DeleteAsync(testUser)).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.IsEmailConfirmedAsync(testUser)).ReturnsAsync(true);
            userManager.Setup(x => x.ResetPasswordAsync(testUser, "testToken", "testPw")).ReturnsAsync(IdentityResult.Success);

            HttpContextAccessor contextAccessor = new HttpContextAccessor();
            contextAccessor.HttpContext = new DefaultHttpContext();

            ILoggerFactory NullLoggerFactory = new NullLoggerFactory();

            var emailSender = new Mock<IEmailSender>();
            emailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask).Verifiable();
            var emailHelper = new EmailHelper(new LinkMock(), emailSender.Object, NullLoggerFactory.CreateLogger<EmailHelper>());

            passwordService = new PasswordService(userManager.Object, contextAccessor, NullLoggerFactory.CreateLogger<PasswordService>(), emailHelper);
        }

        [Test]
        public async Task ForgotPasswordTest()
        {
            var forgotPassword = new ForgotPasswordModelDTO();
            forgotPassword.Email = "test@email.com";
            var result = await passwordService.ForgotPassword(forgotPassword);
            Assert.That(result, Is.EqualTo("If your email is registered, you will receive a password reset link."));
        }

        public async Task ResetPasswordTest()
        {
            var resetPassword = new ResetPasswordModelDTO();
            resetPassword.Email = "test@email.com";
            resetPassword.Token = "testToken";
            var result = await passwordService.ResetPassword(resetPassword);
            Assert.True(result);
        }
    }
}
