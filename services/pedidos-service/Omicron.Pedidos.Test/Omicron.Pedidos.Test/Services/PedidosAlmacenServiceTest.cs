// <summary>
// <copyright file="PedidosAlmacenServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapFile;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidosAlmacenServiceTest : BaseTest
    {
        private IPedidosAlmacenService pedidosAlmacen;

        private Mock<IConfiguration> configuration;

        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacen")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            var mockSapFile = new Mock<ISapFileService>();

            this.configuration = new Mock<IConfiguration>();
            this.configuration.SetupGet(x => x[It.Is<string>(s => s == "OmicronFilesAddress")]).Returns("http://localhost:5002/");

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosAlmacen = new PedidosAlmacenService(this.pedidosDao, mockSapFile.Object, this.configuration.Object);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForAlmacen()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForAlmacen();

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateUserOrders()
        {
            // arrange
            var listorders = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Status = "Almacenado" },
            };

            // act
            var result = await this.pedidosAlmacen.UpdateUserOrders(listorders);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForDelivery()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForDelivery();

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForInvoice()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForInvoice();

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForPackages()
        {
            // arrange
            var dict = new Dictionary<string, string>
            {
                { "status", "Empaquetado,Enviado" },
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForPackages(dict);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateSentOrders()
        {
            // arrange
            var listUserOrders = new List<UserOrderModel>
            {
                new UserOrderModel { InvoiceId = 100, StatusInvoice = "Enviado" },
            };

            // act
            var result = await this.pedidosAlmacen.UpdateSentOrders(listUserOrders);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetAlmacenGraphData()
        {
            // arrange
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            var today = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            var dict = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, $"{yesterday}-{today}" },
            };

            // act
            var result = await this.pedidosAlmacen.GetAlmacenGraphData(dict);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetUserOrderByDeliveryOrder()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.pedidosAlmacen.GetUserOrderByDeliveryOrder(listIds);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateinvoicePdf()
        {
            var details = new List<int> { 100 };
            var type = "invoice";

            var dictResponse = new Dictionary<string, string>
            {
                { "100", "Ok-C:\\algo" },
            };

            var response = new ResultModel
            {
                Code = 200,
                Success = true,
                Response = JsonConvert.SerializeObject(dictResponse),
            };

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            var pedidoServiceLocal = new PedidosAlmacenService(this.pedidosDao, mockSapFile.Object, this.configuration.Object);

            // act
            var result = await pedidoServiceLocal.CreatePdf(type, details);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
