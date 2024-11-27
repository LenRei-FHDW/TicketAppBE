using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketAPI.Data.Models;
using TicketAPI.Services.Helper;
using TicketAPI.Services.Scoped;
using TicketAPITests.Mocks;

namespace TicketAPITests.ServiceTests
{
    internal class FileServiceTests
    {
        private FileService fileService;
        private string testPath;

        [SetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            testPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "/TicketApiTests");
            Directory.CreateDirectory(testPath);

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(x => x.ContentRootPath).Returns(testPath);

            ILoggerFactory NullLoggerFactory = new NullLoggerFactory();

            fileService = new FileService(env.Object, configuration, NullLoggerFactory.CreateLogger<FileService>());

        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(testPath, true);
        }

        [Test]
        public async Task SaveFileTest()
        {
            var file = "TestData/Test.txt";
            using var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            await fileService.SaveFileAsync(formFile);
            Assert.Pass();
        }

        [Test]
        public async Task SaveFileFailTest()
        {
            //Assert.Throws<ArgumentNullException>(delegate { fileService.SaveFileAsync(null); });
        }
    }
}
