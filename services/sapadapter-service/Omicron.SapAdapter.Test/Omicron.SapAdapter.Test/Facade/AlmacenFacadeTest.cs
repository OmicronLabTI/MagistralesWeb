// <summary>
// <copyright file="AlmacenFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
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

            mockService
                .Setup(m => m.GetOrders(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockService
                .Setup(m => m.GetMagistralScannedData(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockService
                .Setup(m => m.GetLineScannedData(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockService
                .Setup(m => m.GetCompleteDetail(It.IsAny<int>()))
                .Returns(Task.FromResult(response));

            mockService
                .Setup(m => m.GetDeliveryBySaleOrderId(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            mockDelivery
                .Setup(m => m.GetDelivery(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockInvoice
                .Setup(m => m.GetInvoice(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockInvoice
                .Setup(m => m.GetInvoiceProducts(It.IsAny<int>()))
                .Returns(Task.FromResult(response));

            mockInvoice
                .Setup(m => m.GetDeliveryScannedData(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockInvoice
                .Setup(m => m.GetMagistralProductInvoice(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockInvoice
                .Setup(m => m.GetLineProductInvoice(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            this.almacenFacade = new SapAlmacenFacade(mapper, mockService.Object, mockDelivery.Object, mockInvoice.Object);
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
    }
}
