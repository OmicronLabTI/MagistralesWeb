// <summary>
// <copyright file="SapServiceTest.cs" company="Axity">
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
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class SapServiceTest : BaseTest
    {
        private ISapService sapService;

        private ISapDao sapDao;

        private IUsersService userService;

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
            this.context.Users.AddRange(this.GetSapUsers());
            this.context.DetalleFormulaModel.AddRange(this.GetDetalleFormula());
            this.context.ItemWarehouseModel.AddRange(this.GetItemWareHouse());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.BatchTransacitions.AddRange(this.GetBatchTransacitions());
            this.context.BatchesTransactionQtyModel.AddRange(this.GetBatchesTransactionQtyModel());

            this.context.SaveChanges();
            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();

            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockPedidoService
                .Setup(m => m.GetFabricationOrders(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(this.GetResultDtoGetUsersById()));

            this.sapDao = new SapDao(this.context);
            this.sapService = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersFechaIni()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
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
        public async Task GetOrdersFechaFin()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dates) },
                { ServiceConstants.Status, "O" },
                { ServiceConstants.Qfb, "abc" },
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
        public async Task GetOrdersId()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, "100" },
                { ServiceConstants.Status, "O" },
                { ServiceConstants.Qfb, "abc" },
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
        public async Task GetOrdersStatus()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dates) },
                { ServiceConstants.Status, "O" },
                { ServiceConstants.Qfb, "abc" },
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
        public async Task GetOrdersTodayById()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, "100" },
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

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetPedidoWithDetail()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetPedidoWithDetail(listIds);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetProdOrderByOrderItem()
        {
            // arrange
            var listIds = new List<string> { "100-Buscapina" };

            // act
            var result = await this.sapService.GetProdOrderByOrderItem(listIds);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrderFormula()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetOrderFormula(listIds, true);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrderFormulaList()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetOrderFormula(listIds, false);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetComponents()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", "Abc,pirina" },
            };

            // act
            var result = await this.sapService.GetComponents(paramsDict);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetComponentsChipDescription()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", "pirina" },
            };

            // act
            var result = await this.sapService.GetComponents(paramsDict);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetComponentsNoData()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", "qwerty" },
            };

            // act
            var result = await this.sapService.GetComponents(paramsDict);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetComponentsNoChips()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
            };

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await this.sapService.GetComponents(paramsDict));
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetBatchesComponents()
        {
            // arrange
            var ordenId = 100;

            // act
            var result = await this.sapService.GetBatchesComponents(ordenId);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetlLastIsolatedProductionOrderId()
        {
            // arrange
            var productId = "Abc Aspirina";
            var uniqueId = "token";

            // act
            var result = await this.sapService.GetlLastIsolatedProductionOrderId(productId, uniqueId);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
