// <summary>
// <copyright file="SapDxpServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class SapDxpServiceTest : BaseTest
    {
        private ISapDxpService sapDxpService;

        private ISapDao sapDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalDxpService")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.SalesPersonModel.AddRange(this.GetSalesPerson());
            this.context.DeliveryDetailModel.AddRange(this.GetDeliveryDetail());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.CatalogProductModel.AddRange(this.GetCatalogProductModel());
            this.context.InvoiceHeaderModel.AddRange(this.GetInvoiceHeader());

            this.context.SaveChanges();
            var mockLog = new Mock<ILogger>();
            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapDxpService = new SapDxpService(this.sapDao, mockRedis.Object);
        }

        /// <summary>
        /// gets the possible orders active for dxp project.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersActive()
        {
            // arrange
            var ordersId = new List<int>
            {
                100,
            };

            // act
            var result = await this.sapDxpService.GetOrdersActive(ordersId);

            Assert.IsNotNull(result);
        }
    }
}
