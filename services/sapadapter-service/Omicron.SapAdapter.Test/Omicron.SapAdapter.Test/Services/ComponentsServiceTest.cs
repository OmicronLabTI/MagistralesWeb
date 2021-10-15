// <summary>
// <copyright file="ComponentsServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ComponentsServiceTest : BaseTest
    {
        private ISapDao sapDao;

        private DatabaseContext context;

        private IComponentsService componentService;

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
            this.context.SaveChanges();

            var mockRedis = new Mock<IRedisService>();
            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            mockRedis
                .Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.componentService = new ComponentsService(this.sapDao, mockRedis.Object);
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

            var service = new ComponentsService(this.sapDao, redis.Object);

            // act
            var result = await service.GetMostCommonComponents();

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetMostCommonComponents()
        {
            // arrange
            var components = new List<ComponentsRedisModel>
            {
                new ComponentsRedisModel { ItemCode = "Cápsula 12ML", Total = 1 },
            };

            var mockLog = new Mock<ILogger>();
            var redis = new Mock<IRedisService>();
            redis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(components)));

            var service = new ComponentsService(this.sapDao, redis.Object);

            // act
            var result = await service.GetMostCommonComponents();

            Assert.IsNotNull(result);
        }
    }
}
