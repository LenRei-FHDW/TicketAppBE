using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    internal class ProductServiceTests
    {
        private ApplicationUser testUser;
        private ProductService productService;
        private Guid productId;
        private Product testProduct;

        [SetUp]
        public void Setup()
        {
            testUser = new ApplicationUser { UserName = "test", Id = "123", Email = "test@email.com" };

            ILoggerFactory NullLoggerFactory = new NullLoggerFactory();

            testProduct = new Product();
            testProduct.Name = "test";
            testProduct.Description = "test";
            testProduct.Price = 100;
            testProduct.ImageName = "test";
            productId = Guid.NewGuid();
            testProduct.ProductId = productId;
            testProduct.Name = "test";

            var productRepository = new Mock<IProductRepository>();
            productRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Product> { testProduct });

            var repository = new Mock<IRepository<Product, Guid>>();
            repository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(testProduct);
            repository.Setup(x => x.AddAsync(It.IsAny<Product>())).ReturnsAsync(testProduct);
            repository.Setup(x => x.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(testProduct);

            var profile = new MappingProfile();
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(profile); });
            var mapper = new Mapper(config);

            var optionsBuilder = new DbContextOptionsBuilder<TicketApiDbContext>();
            var context = new Mock<TicketApiDbContext>(optionsBuilder.Options);

            productService = new ProductService(repository.Object, productRepository.Object, mapper, NullLoggerFactory.CreateLogger<ProductService>());
        }

        [Test]
        public async Task GetAllProductsTest()
        {
            var result = await productService.GetAllProductsAsync();
            Assert.That(result.Count() > 0);
        }

        [Test]
        public async Task GetProductByIdTest()
        {
            var result = await productService.GetProductById(productId);
            Assert.That(result.Name.Equals("test"));
        }

        [Test]
        public async Task AddProductTest()
        {
            var create = new ProductCreateDTO();
            create.Name = "test";
            var result = await productService.AddProduct(create,"test", "noImage");
            Assert.That(result.Name.Equals("test"));
        }

        [Test]
        public async Task EditProductTest()
        {
            var edit = new ProductEditDTO();
            edit.Name = "test";
            edit.Description = "test";
            edit.Price = 100;
            edit.ImageName = "test";
            edit.ProductId = productId;
            var result = await productService.EditProduct(edit);
            Assert.That(result.Name.Equals("test"));
        }

        [Test]
        public async Task DeleteProductTest()
        {
            await productService.DeleteProduct(productId);
            Assert.Pass();
        }
    }
}
