using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;
using TicketAPI.Services.Helper.Mappings;
using TicketAPI.Services.Scoped;
using TicketAPITests.Mocks;

namespace TicketAPITests.ServiceTests
{
    internal class OrderServiceTests
    {
        ApplicationUser testUser;
        Guid orderId;
        Order testOrder;
        OrderService orderService;

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

            ILoggerFactory NullLoggerFactory = new NullLoggerFactory();


            var testOrderItem = new OrderItem();
            testOrderItem.Quantity = 1;
            testOrder = new Order();
            orderId = Guid.NewGuid();
            testOrder.OrderId = orderId;
            testOrder.ApplicationUser = testUser;
            testOrderItem.Order = testOrder;

            var optionsBuilder = new DbContextOptionsBuilder<TicketApiDbContext>();
            var context = new Mock<TicketApiDbContext>(optionsBuilder.Options);

            var ordertRepository = new Mock<OrderRepository>(context);
            ordertRepository.Setup(x => x.GetByIdAsynchLoadEager(orderId)).ReturnsAsync(testOrder);

            var repository = new Mock<IRepository<OrderItem, Guid>>();
            repository.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(testOrderItem);
            repository.Setup(x => x.AddRangeAsync(new List<OrderItem> { testOrderItem })).Returns(Task.CompletedTask);

            var profile = new MappingProfile();
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(profile); });
            var mapper = new Mapper(config);

            var emailSender = new Mock<IEmailSender>();
            emailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask).Verifiable();
            var emailHelper = new EmailHelper(new LinkMock(), emailSender.Object, NullLoggerFactory.CreateLogger<EmailHelper>());

            orderService = new OrderService(ordertRepository.Object, repository.Object, context.Object, mapper, NullLoggerFactory.CreateLogger<OrderService>(), userManager.Object, emailHelper);
        }

        [Test]
        public async Task GetOrderTest()
        {
            // Braucht OrderRepositoryInterface
            var result = await orderService.GetOrder("test@email.com",orderId, false);
            Assert.That(result.OrderItems.Count() > 0);
        }

        [Test]
        public async Task CreateOrderTest()
        {
            // Braucht OrderRepositoryInterface
            OrderItemPostDTO post = new() { ProductId = orderId, Quantity = 1};

            var result = await orderService.CreateNewOrder("test@email.com", new List<OrderItemPostDTO>() { post });
            Assert.That(result.OrderItems.Count() > 0);
        }
    }
}
