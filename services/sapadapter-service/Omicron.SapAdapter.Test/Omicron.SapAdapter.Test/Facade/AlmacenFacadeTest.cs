// <summary>
// <copyright file="AlmacenFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
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
            var mockOrderDxp = new Mock<IAlmacenOrderDxpService>();

            mockService.SetReturnsDefault(Task.FromResult(response));
            mockDelivery.SetReturnsDefault(Task.FromResult(response));
            mockInvoice.SetReturnsDefault(Task.FromResult(response));
            mockAdvance.SetReturnsDefault(Task.FromResult(response));
            mockOrdersdDoctor.SetReturnsDefault(Task.FromResult(response));
            mockOrderDxp.SetReturnsDefault(Task.FromResult(response));

            this.almacenFacade = new SapAlmacenFacade(mapper, mockService.Object, mockDelivery.Object, mockInvoice.Object, mockAdvance.Object, mockOrdersdDoctor.Object, mockOrderDxp.Object);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersDetails()
        {
            var orderId = 1;
            var response = await this.almacenFacade.GetOrdersDetails(orderId);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataMagistral()
        {
            var code = "75000-1000";
            var response = await this.almacenFacade.GetScannedDataMagistral(code);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataLine()
        {
            var orderId = 75000;
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedDataLinea(code, orderId);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetProductsWithCodeBars()
        {
            var response = await this.almacenFacade.GetProductsWithCodeBars();

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders models.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersById()
        {
            var response = await this.almacenFacade.GetOrdersByIds(new List<int>());
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.UserError, Is.Empty);
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

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersDeliveryDetail()
        {
            var dictionary = 1;
            var response = await this.almacenFacade.GetOrdersDeliveryDetail(dictionary);

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoiceByFilters()
        {
            var dictionary = new Dictionary<string, string>();
            var response = await this.almacenFacade.GetInvoiceByFilters(dictionary);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoiceDetail()
        {
            var response = await this.almacenFacade.GetInvoiceDetail(123);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoiceProducts()
        {
            var response = await this.almacenFacade.GetInvoiceProducts(10, "Distribucion", null);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataRemision()
        {
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedDataRemision(code);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataRemisionMg()
        {
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedDataRemisionMg(code);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataRemisionLn()
        {
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedDataRemisionLn(code);

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetScannedDataFactura()
        {
            var code = "750001000";
            var response = await this.almacenFacade.GetScannedDataFactura(code);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders detail.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDetallePedidoPiezas()
        {
            var orderId = 75000;
            var response = await this.almacenFacade.GetDetailOrder(orderId);

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryParties()
        {
            var response = await this.almacenFacade.GetDeliveryParties();

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveries()
        {
            var response = await this.almacenFacade.GetDeliveries(new List<int>());

            Assert.That(response, Is.Not.Null);
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

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task AdvanceLookUp()
        {
            var response = await this.almacenFacade.AdvanceLookUp(new Dictionary<string, string>());

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetProductsDelivery()
        {
            var response = await this.almacenFacade.GetProductsDelivery(string.Empty);

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the get Almacen Orders By Doctor.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrderdetail()
        {
            var response = await this.almacenFacade.GetOrderdetail(123);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
        }

        /// <summary>
        /// Test the get Almacen Orders By Doctor.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersByDxpId()
        {
            var response = await this.almacenFacade.SearchAlmacenOrdersByDxpId(new Dictionary<string, string>());

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
        }

        /// <summary>
        /// Test the get Almacen Orders By Doctor.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersDetailsByDxpId()
        {
            var response = await this.almacenFacade.SearchAlmacenOrdersDetailsByDxpId(new DoctorOrdersSearchDeatilDto());

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
        }

        /// <summary>
        /// Test the get Almacen Orders By Doctor.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoicesByIds()
        {
            var response = await this.almacenFacade.GetInvoicesByIds(new List<int> { 123 });

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
        }
    }
}
