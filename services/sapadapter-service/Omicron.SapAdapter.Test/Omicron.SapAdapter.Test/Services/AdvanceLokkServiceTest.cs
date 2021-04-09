// <summary>
// <copyright file="AdvanceLokkServiceTest.cs" company="Axity">
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
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AdvanceLokkServiceTest : BaseTestAdvancedLookUp
    {
        private IAdvanceLookService advanceLookService;

        private ISapDao sapDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAdvancedLook")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.DeliveryDetailModel.AddRange(this.GetDeliveryDetail());
            this.context.DeliverModel.AddRange(this.DeliveryModel());
            this.context.InvoiceHeaderModel.AddRange(this.GetInvoiceHeader());
            this.context.InvoiceDetailModel.AddRange(this.GetInvoiceDetails());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.Repartidores.AddRange(this.GetRepartidores());
            this.context.AsesorModel.AddRange(this.GetAsesorModel());
            this.context.SaveChanges();

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            var userMock = new Mock<IUsersService>();

            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<List<int>>()))
                .Returns(Task.FromResult(this.GetResultGetAdvancedModelAlmacen()));

            userMock
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsers()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.advanceLookService = new AdvanceLookService(this.sapDao, mockPedidoService.Object, mockAlmacen.Object, userMock.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        /// [TestCase("0")]
        /// [TestCase("145")]
        /// [TestCase("84434")]
        /// [TestCase("74709")]
        /// [TestCase("115010")]
        /// [TestCase("84458")]
        /// [TestCase("74728")]
        /// [TestCase("84473")]
        /// [TestCase("74751")]
        /// [TestCase("115024")]
        /// [TestCase("115025")]
        /// [TestCase("114966")]
        /// [TestCase("84508")]
        [Test]
        [TestCase("0")]
        [TestCase("145")]
        [TestCase("84434")]
        [TestCase("84458")]
        [TestCase("84473")]
        [TestCase("84508")]
        public async Task GetCardsByOrder(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("74709")]
        [TestCase("74728")]
        [TestCase("74751")]
        [TestCase("74746")]
        public async Task GetCardsByDelivery(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("115010")]
        [TestCase("115024")]
        [TestCase("115025")]
        [TestCase("114966")]
        public async Task GetCardsByInvoice(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="medico">the doctor.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase(ServiceConstants.SaleOrder, "Medico,A")]
        [TestCase(ServiceConstants.Delivery, "Medico,B")]
        [TestCase(ServiceConstants.Invoice, "Medico,B")]
        public async Task GetCardsByDoctor(string type, string medico)
        {
            // arrange
            var dates = new DateTime(2021, 03, 06);
            var dateFinal = new DateTime(2021, 04, 08);
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates.ToString("dd/MM/yyyy"), dateFinal.ToString("dd/MM/yyyy")) },
                { ServiceConstants.Doctor, medico },
                { ServiceConstants.Type, type },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);

            Assert.IsNotNull(result);
        }
    }
}
