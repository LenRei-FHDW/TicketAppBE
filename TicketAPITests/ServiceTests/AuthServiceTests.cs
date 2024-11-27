using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
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
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TicketAPITests.ServiceTests
{
    internal class AuthServiceTests
    {
        private AuthService authService;
        private ApplicationUser testUser;

        [SetUp]
        public void Setup()
        {
            testUser = new ApplicationUser { UserName = "test", Id = "123", Email = "test@email.com", EmailConfirmed = true };

            var store = new Mock<IUserStore<ApplicationUser>>();
            store.Setup(x => x.FindByIdAsync("123", CancellationToken.None))
                .ReturnsAsync(testUser);
            var userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.FindByEmailAsync("test@email.com")).ReturnsAsync(testUser);
            userManager.Setup(x => x.CheckPasswordAsync(testUser, "testPw")).ReturnsAsync(true);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "testPw")).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(testUser)).ReturnsAsync("testToken");
            userManager.Setup(x => x.DeleteAsync(testUser)).ReturnsAsync(IdentityResult.Success);

            HttpContextAccessor contextAccessor = new HttpContextAccessor();
            contextAccessor.HttpContext = new DefaultHttpContext();

            ILoggerFactory NullLoggerFactory = new NullLoggerFactory();

            var tokenGenerator = new Mock<ITokenGenerator>();
            tokenGenerator.Setup(x => x.GenerateToken(testUser)).ReturnsAsync("testToken");
            var emailSender = new Mock<IEmailSender>();
            emailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask).Verifiable();
            var emailHelper = new EmailHelper(new LinkMock(), emailSender.Object, NullLoggerFactory.CreateLogger<EmailHelper>());

            authService = new AuthService(userManager.Object, contextAccessor, tokenGenerator.Object, NullLoggerFactory.CreateLogger<AuthService>(), emailHelper);
        }

        [Test]
        public async Task LoginTest()
        {
            LoginModelDTO loginModel = new LoginModelDTO();
            loginModel.Email = "test@email.com";
            loginModel.Password = "testPw";
            var result = await authService.LoginAsync(loginModel);
            Assert.That(result.Token.Equals("testToken") && result.IsEmailConfirmed);
        }

        [Test]
        public async Task LoginFailTest()
        {
            LoginModelDTO loginModel = new LoginModelDTO();
            loginModel.Email = "test@email.com";
            loginModel.Password = "wrongtestPw";
            var result = await authService.LoginAsync(loginModel);
            Assert.That(result == null);
        }

        [Test]
        public async Task RegisterTest()
        {
            RegisterModelDTO registerModel = new RegisterModelDTO();
            registerModel.FirstName = "Test";
            registerModel.LastName = "Testmanm";
            registerModel.Email = "test@email.com";
            registerModel.Password = "testPw";
            registerModel.RepeatPassword = "testPw";
            var result = await authService.Register(registerModel);
            Assert.That(result.Succeeded && result.SamePassword);
        }

        [Test]
        public async Task RegisterFailTest()
        {
            RegisterModelDTO registerModel = new RegisterModelDTO();
            registerModel.FirstName = "Test";
            registerModel.LastName = "Testmanm";
            registerModel.Email = "test@email.com";
            registerModel.Password = "testPw";
            registerModel.RepeatPassword = "wrongTestPw";
            var result = await authService.Register(registerModel);
            Assert.That(!result.SamePassword);
        }
    }
}
