// <summary>
// <copyright file="SapServiceTest.cs" company="Axity">
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
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class SapServiceTest : BaseTest
    {
        private ISapService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.AsesorModel.Add(this.GetAsesorModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.ProductoModel.AddRange(this.GetProductoModel());

            this.context.SaveChanges();
            var mockPedidoService = new Mock<IPedidosService>();

            this.sapDao = new SapDao(this.context);
            this.sapService = new SapService(this.sapDao, mockPedidoService.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersToday()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FilterDate, ServiceConstants.Today },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersTwoWeeks()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FilterDate, ServiceConstants.TwoWeeks },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersMoth()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FilterDate, ServiceConstants.Month },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersWeek()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FilterDate, ServiceConstants.Week },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersOtherDate()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FilterDate, string.Empty },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// the order detail.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrderDetail()
        {
            // arrange
            var docId = 100;

            // act
            var result = await this.sapService.GetOrderDetails(docId);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
