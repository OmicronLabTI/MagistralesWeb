// <summary>
// <copyright file="ComponentsServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ComponentsServiceTest : BaseTest
    {
        private ISapDao sapDao;

        private DatabaseContext context;

        private IComponentsService componentService;

        private Mock<IUsersService> mockUserService;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalComponent")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.CatalogProductModel.AddRange(this.GetCatalogProductModel());
            this.context.SaveChanges();

            var mockRedis = new Mock<IRedisService>();
            var mockLog = new Mock<ILogger>();
            this.mockUserService = new Mock<IUsersService>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            mockRedis
                .Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            this.mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetUsersById()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.componentService = new ComponentsService(this.sapDao, mockRedis.Object, this.mockUserService.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetMostCommonComponentsEmpty()
        {
            // arrange
            var mockLog = new Mock<ILogger>();
            var redis = new Mock<IRedisService>();
            redis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            var service = new ComponentsService(this.sapDao, redis.Object, this.mockUserService.Object);

            // act
            var result = await service.GetMostCommonComponents(new Dictionary<string, string>());

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="type">Type.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", "detailOrder")]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", "inputRequest")]
        [TestCase("", "detailOrder")]
        public async Task GetMostCommonComponents(string userId, string type)
        {
            // arrange
            var components = new List<ComponentsRedisModel>
            {
                new ComponentsRedisModel { ItemCode = "Cápsula 12ML", Total = 1 },
            };

            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "catalogGroup", "DZ" },
                { "type", type },
            };

            if (!string.IsNullOrEmpty(userId))
            {
                paramsDict["userId"] = userId;
            }

            var mockLog = new Mock<ILogger>();
            var redis = new Mock<IRedisService>();
            redis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(components)));

            var service = new ComponentsService(this.sapDao, redis.Object, this.mockUserService.Object);

            // act
            var result = await service.GetMostCommonComponents(new Dictionary<string, string>());

            Assert.That(result, Is.Not.Null);
        }
    }
}
