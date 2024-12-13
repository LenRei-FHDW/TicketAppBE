using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TicketAPI.Services.Helper;
using TicketAPI.Services.Scoped;
using TicketAPITests.Mocks;

namespace TicketAPITests.ServiceTests
{
    internal class EmailHelperTests
    {
        private EmailHelper _emailHelper;
        private Mock<IEmailSender> _emailSenderMock;

        [SetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            _emailSenderMock = new Mock<IEmailSender>();
            _emailSenderMock.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask).Verifiable();
            var _emailSender = _emailSenderMock.Object;
            var env = new Mock<IWebHostEnvironment>();
            env.Setup(m => m.ContentRootPath).Returns("");
            env.Object.ContentRootPath = "test";

            ILoggerFactory NullLoggerFactory = new NullLoggerFactory();

            _emailHelper = new EmailHelper(new LinkMock(), _emailSender, NullLoggerFactory.CreateLogger<EmailHelper>(), env.Object, configuration);
        }

        [Test]
        public async Task GenerateVerificationEmailTest()
        {
            _emailHelper.GenerateVerificationEmail("test", new DefaultHttpContext(), "test", "test", "test");
            _emailSenderMock.Verify();
            Assert.Pass();
        }


        [Test]
        public async Task GenerateResetEmail()
        {
            _emailHelper.GenerateResetEmail("test", "test", "test", "test");
            _emailSenderMock.Verify();
            Assert.Pass();
        }
    }
}
