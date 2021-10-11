// <summary>
// <copyright file="AlmacenFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Services.Mapping;
    using Omicron.SapAdapter.Services.Sap;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class AlmacenFacadeTest : BaseTest
    {
        private SapAlmacenFacade almacenFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockService = new Mock<ISapAlmacenService>();
            var mockDelivery = new Mock<ISapAlmacenDeliveryService>();
            var mockInvoice = new Mock<ISapInvoiceService>();
            var mockAdvance = new Mock<IAdvanceLookService>();
            var mockOrdersdDoctor = new Mock<IAlmacenOrderDoctorService>();

            mockService.SetReturnsDefault(Task.FromResult(response));
            mockDelivery.SetReturnsDefault(Task.FromResult(response));
            mockInvoice.SetReturnsDefault(Task.FromResult(response));
            mockAdvance.SetReturnsDefault(Task.FromResult(response));
            mockOrdersdDoctor.SetReturnsDefault(Task.FromResult(response));

            this.almacenFacade = new SapAlmacenFacade(mapper, mockService.Object, mockDelivery.Object, mockInvoice.Object, mockAdvance.Object, mockOrdersdDoctor.Object);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetPedidos()
        {
            var dictionary = new Dictionary<string, string>();
            var response = await this.almacenFacade.GetOrders(dictionary);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataMagistral()
        {
            var type = "magistral";
            var code = "75000-1000";
            var response = await this.almacenFacade.GetScannedData(type, code);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataLine()
        {
            var type = "line";
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedData(type, code);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetCompleteDetail()
        {
            var order = 1000;
            var response = await this.almacenFacade.GetCompleteDetail(order);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryBySaleOrderId()
        {
            var order = new List<int>();
            var response = await this.almacenFacade.GetDeliveryBySaleOrderId(order);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDelivery()
        {
            var dictionary = new Dictionary<string, string>();
            var response = await this.almacenFacade.GetDelivery(dictionary);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoice()
        {
            var dictionary = new Dictionary<string, string>();
            var response = await this.almacenFacade.GetInvoice(dictionary);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoiceProducts()
        {
            var response = await this.almacenFacade.GetInvoiceProducts(10);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataRemision()
        {
            var type = "remision";
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedData(type, code);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataRemisionMg()
        {
            var type = "remisionmg";
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedData(type, code);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataRemisionLn()
        {
            var type = "remisionln";
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedData(type, code);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoiceHeaders()
        {
            // arrange
            var data = new InvoicePackageSapLookDto();

            // act
            var response = await this.almacenFacade.GetInvoiceHeader(data);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataFactura()
        {
            var type = "factura";
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedData(type, code);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetSapIds()
        {
            var listids = new List<int>();
            var response = await this.almacenFacade.GetSapIds(listids);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task AlmacenGraphCount()
        {
            var listids = new Dictionary<string, string>();
            var response = await this.almacenFacade.AlmacenGraphCount(listids);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryParties()
        {
            var response = await this.almacenFacade.GetDeliveryParties();

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveries()
        {
            var response = await this.almacenFacade.GetDeliveries(new List<int>());

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetCancelledInvoices()
        {
            var days = 10;
            var response = await this.almacenFacade.GetCancelledInvoices(days);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task AdvanceLookUp()
        {
            var response = await this.almacenFacade.AdvanceLookUp(new Dictionary<string, string>());

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetProductsDelivery()
        {
            var response = await this.almacenFacade.GetProductsDelivery(string.Empty);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the get Almacen Orders By Doctor.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersByDoctor()
        {
            var response = await this.almacenFacade.SearchAlmacenOrdersByDoctor(new Dictionary<string, string>());

            Assert.IsNotNull(response);
        }
    }
}
