using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.Helper;

namespace TicketAPITests.ServiceTests
{
    public class JwtGeneratorTests
    {
        private JwtGenerator _jwtGenerator;
        private ApplicationUser testUser;

        [SetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            testUser = new ApplicationUser { UserName = "test", Id = "123", Email = "test@email.com" };

            var store = new Mock<IUserStore<ApplicationUser>>();
            store.Setup(x => x.FindByIdAsync("123", CancellationToken.None))
                .ReturnsAsync(testUser);
            var userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.GetRolesAsync(testUser)).ReturnsAsync(new List<string> { "Admin" });

            _jwtGenerator = new JwtGenerator(configuration, userManager.Object);
        }

        [Test]
        public async Task GenerateTokenTestAsync()
        {
            string token = await _jwtGenerator.GenerateToken(testUser);
            Assert.That(!string.IsNullOrEmpty(token));
        }
    }
}