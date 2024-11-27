using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.Helper;
using TicketAPI.Services.Scoped;
using TicketAPITests.Mocks;

namespace TicketAPITests.ServiceTests
{
    internal class UserServiceTests
    {
        private UserService userService;
        private ApplicationUser testUser;

        [SetUp]
        public void Setup()
        {
            testUser = new ApplicationUser { UserName = "test", Id = "123", Email = "test@email.com", FirstName = "Test", LastName = "Testmann" };

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetUserWithAddressAsync("123")).ReturnsAsync(testUser);
            
            userService = new UserService(userRepository.Object);
        }

        [Test]
        public async Task GetUserDataAsyncTest()
        {
            var result = await userService.GetUserDataAsync("123");
            Assert.That(result.LastName.Equals("Testmann"));
        }

        [Test]
        public async Task UpdateUserDataTest()
        {
            UserDataEditDTO edit = new UserDataEditDTO();
            var result = await userService.UpdateUserDataAsync(testUser.Id, edit);
            Assert.Pass();
        }
    }
}
