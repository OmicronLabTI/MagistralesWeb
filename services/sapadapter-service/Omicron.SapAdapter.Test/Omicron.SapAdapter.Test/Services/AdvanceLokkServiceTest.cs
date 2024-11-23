// <summary>
// <copyright file="AdvanceLokkServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AdvanceLokkServiceTest : BaseTestAdvancedLookUp
    {
        private IAdvanceLookService advanceLookService;

        private ISapDao sapDao;

        private DatabaseContext context;

        private Mock<ICatalogsService> catalogService;

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
            this.context.ClientCatalogModel.AddRange(this.GetDoctorsModels());
            this.context.SaveChanges();

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            var userMock = new Mock<IUsersService>();

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
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

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Value = "10" },
                new ParametersModel { Id = 2, Value = "Codigo", Field = "CardCodeResponsibleMedic" },
            };

            this.catalogService = new Mock<ICatalogsService>();
            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModel(parameters)));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<PaymentsByTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments)
            .Verifiable("Notification was not sent.");

            mockMediator.Setup(m => m.Send(It.IsAny<DoctorDeliveryAddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorDeliveryAddressModel>())
            .Verifiable("Notification was not sent.");

            this.sapDao = new SapDao(this.context, mockLog.Object);

            this.advanceLookService = new AdvanceLookService(this.sapDao, mockPedidoService.Object, mockAlmacen.Object, userMock.Object, this.catalogService.Object, mockRedis.Object, mockMediator.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("0")]
        [TestCase("145")]
        public async Task GetCardsWhenDontExistsDocNum(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardInvoice, Is.Empty);
            Assert.That(cards.CardDelivery, Is.Empty);
            Assert.That(cards.CardDistribution, Is.Empty);
            Assert.That(cards.CardOrder, Is.Empty);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <param name="orders">the total orders.</param>
        /// <param name="deliverys">the total deliverys.</param>
        /// <param name="invoices">the total invoices.</param>
        /// <param name="distribution">the total distribuitions.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("84434", 0, 0, 2, 0)]
        [TestCase("84458", 0, 1, 0, 0)]
        [TestCase("84473", 0, 0, 0, 1)]
        [TestCase("84508", 1, 0, 0, 0)]
        public async Task GetCardsByOrder(string docNum, int orders, int deliverys, int invoices, int distribution)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(invoices.Equals(cards.CardInvoice.Count));
            Assert.That(deliverys.Equals(cards.CardDelivery.Count));
            Assert.That(distribution.Equals(cards.CardDistribution.Count));
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
        public async Task GetCardsByDelivery(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("74746")]
        public async Task GetCardsByDeliveryCancelled(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardDelivery, Is.Not.Empty);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("115010")]
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
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardInvoice, Is.Not.Empty);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("115024")]
        public async Task GetCardsByPackage(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardDistribution, Is.Not.Empty);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNum">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("115025")]
        public async Task GetCardsByInvoiceCancelled(string docNum)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNum },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardInvoice, Is.Not.Empty);
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

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="medico">the doctor.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase(ServiceConstants.SaleOrder, "Doc,ff")]
        [TestCase(ServiceConstants.Delivery, "Doc,BB")]
        [TestCase(ServiceConstants.Invoice, "Doc,rr")]
        public async Task GetCardsByWhenDontExistsDoctor(string type, string medico)
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
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardInvoice, Is.Empty);
            Assert.That(cards.CardDelivery, Is.Empty);
            Assert.That(cards.CardDistribution, Is.Empty);
            Assert.That(cards.CardOrder, Is.Empty);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="docNumDxp">the docNum.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("#847822")]
        public async Task GetCardsByDxpId(string docNumDxp)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, docNumDxp },
            };

            // act
            var result = await this.advanceLookService.AdvanceLookUp(dicParams);
            var cards = (CardsAdvancedLook)result.Response;
            Assert.That(result, Is.Not.Null);
            Assert.That(cards.CardOrder, Is.Not.Empty);
        }
    }
}
