// <summary>
// <copyright file="RequestServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Test.Services.Request
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class RequestServiceTest : BaseTest
    {
        private IRequestService requestService;
        private Mock<IUsersService> mockUsersService;
        private Mock<IReportingService> mockReportingService;
        private Mock<ISapAdapterService> mockSapAdapterService;
        private IRequestDao requestDao;
        private DatabaseContext context;
        private string userId = "abc";
        private Mock<IRedisService> redisService;

        /// <summary>
        /// Initialize db.
        /// </summary>
        public void InitializeInMemoryDb()
        {
            var options = CreateNewContextOptions("TemporalRequest");
            this.context = new DatabaseContext(options);

            var request = AutoFixtureProvider.CreateList<RawMaterialRequestModel>(1);
            request[0].Id = 1;
            request[0].Signature = new byte[0];

            request[0].OrderedProducts = AutoFixtureProvider.CreateList<RawMaterialRequestDetailModel>(1);
            request[0].ProductionOrderIds = new List<int> { 1 };
            request[0].OrderedProducts[0].Id = 1;
            request[0].OrderedProducts[0].RequestId = 1;

            this.context.RawMaterialRequests.AddRange(request);
            this.context.RawMaterialRequestDetails.AddRange(request[0].OrderedProducts);
            this.context.RawMaterialRequestOrders.AddRange(request[0].ProductionOrderIds.Select(x => new RawMaterialRequestOrderModel { Id = 1, ProductionOrderId = 4, RequestId = 1 }));

            this.context.SaveChanges();
        }

        /// <summary>
        /// Create mock users.
        /// </summary>
        /// <returns>Mock users.</returns>
        public List<UserModel> GetMockUsers()
        {
            var users = AutoFixtureProvider.CreateList<UserModel>(1);
            users[0].Id = this.userId;
            return users;
        }

        /// <summary>
        /// Create mock production orders.
        /// </summary>
        /// <returns>Mock users.</returns>
        public List<ProductionOrderModel> GetMockProductionOrders()
        {
            var mockList = new List<ProductionOrderModel>();
            mockList.Add(new ProductionOrderModel
            {
                ProductionOrderId = 1,
                Status = "Planificado",
                Details = new List<ProductionOrderComponentModel>()
                {
                    new ProductionOrderComponentModel { ProductId = "1", Description = "Prod 1   1KG", Unit = "KG", Warehouse = "GM", WarehouseQuantity = 10M, RequiredQuantity = 10M, },
                    new ProductionOrderComponentModel { ProductId = "2", Description = "Prod 2   1KG", Unit = "KG", Warehouse = "GM", WarehouseQuantity = 10M, RequiredQuantity = 10M, },
                    new ProductionOrderComponentModel { ProductId = "3", Description = "Prod 3", Unit = "KG", Warehouse = "GM", WarehouseQuantity = 5M, RequiredQuantity = 10M, },
                },
            });
            mockList.Add(new ProductionOrderModel
            {
                ProductionOrderId = 2,
                Status = "Planificado",
                Details = new List<ProductionOrderComponentModel>()
                {
                    new ProductionOrderComponentModel { ProductId = "1", Description = "Prod 1   2KG", Unit = "KG", Warehouse = "GM", WarehouseQuantity = 10M, RequiredQuantity = 5M, },
                    new ProductionOrderComponentModel { ProductId = "3", Description = "Prod 3", Unit = "KG", Warehouse = "PT", WarehouseQuantity = 5M, RequiredQuantity = 10M, },
                },
            });
            mockList.Add(new ProductionOrderModel
            {
                ProductionOrderId = 3,
                Status = "Cancelado",
                Details = new List<ProductionOrderComponentModel>()
                {
                    new ProductionOrderComponentModel { ProductId = "1", Description = "Prod 1", Unit = "KG", Warehouse = "GM", WarehouseQuantity = 10M, RequiredQuantity = 10M, },
                },
            });

            return mockList;
        }

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.InitializeInMemoryDb();
            this.requestDao = new RequestDao(this.context);
            this.mockUsersService = new Mock<IUsersService>();
            this.mockReportingService = new Mock<IReportingService>();
            this.mockSapAdapterService = new Mock<ISapAdapterService>();

            this.mockUsersService.Setup(x => x.GetUsersById(It.IsAny<string[]>())).Returns(Task.FromResult(this.GetMockUsers()));
            this.mockSapAdapterService.Setup(x => x.GetProductionOrdersByCriterial(It.IsAny<List<int>>(), It.IsAny<List<int>>())).Returns(Task.FromResult(this.GetMockProductionOrders()));
            this.mockReportingService.SetReturnsDefault(Task.FromResult((true, string.Empty)));
            this.redisService = new Mock<IRedisService>();
            this.requestService = new RequestService(this.requestDao, this.mockUsersService.Object, this.mockSapAdapterService.Object, this.mockReportingService.Object, this.redisService.Object);
        }

        /// <summary>
        /// Create new raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest_CreateRequest_SuccessResults()
        {
            // arrange
            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.Id = 101;
            request.ProductionOrderIds = new List<int> { 2 };
            request.OrderedProducts = AutoFixtureProvider.CreateList<RawMaterialRequestDetailModel>(3);

            // act
            var response = await this.requestService.CreateRawMaterialRequest(this.userId, request);

            // assert
            this.CheckAction(response, true, 1, 0);
        }

        /// <summary>
        /// Create new raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest_CreateRequestWithProductionOrderIdsNull_SuccessResults()
        {
            // arrange
            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.ProductionOrderIds = new List<int> { };
            request.OrderedProducts = AutoFixtureProvider.CreateList<RawMaterialRequestDetailModel>(3);

            // act
            var response = await this.requestService.CreateRawMaterialRequest(this.userId, request);

            // assert
            this.CheckAction(response, true, 1, 0);
        }

        /// <summary>
        /// Create new raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest_FailOnSubmit_ReturnUSerError()
        {
            // arrange
            this.mockReportingService = new Mock<IReportingService>();
            this.mockReportingService.SetReturnsDefault(Task.FromResult((false, "Ocurrió un error al enviar la solicitud.")));
            this.requestService = new RequestService(this.requestDao, this.mockUsersService.Object, this.mockSapAdapterService.Object, this.mockReportingService.Object, this.redisService.Object);

            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.ProductionOrderIds = new List<int> { 2 };
            request.OrderedProducts = AutoFixtureProvider.CreateList<RawMaterialRequestDetailModel>(3);

            // act
            var response = await this.requestService.CreateRawMaterialRequest(this.userId, request);

            // assert
            ClassicAssert.IsNotNull(response);
            ClassicAssert.AreEqual(ErrorReasonConstants.ErrorToSubmitFile, response.UserError);
        }

        /// <summary>
        /// Create new raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest_RequestAlreadyExists_FailResults()
        {
            // arrange
            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.ProductionOrderIds = new List<int> { 4 };

            // act
            var response = await this.requestService.CreateRawMaterialRequest(this.userId, request);

            // assert
            this.CheckAction(response, true, 0, 1);
        }

        /// <summary>
        /// Create new raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest_UserNotExists_FailResults()
        {
            // arrange
            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.ProductionOrderIds = new List<int> { 1 };

            // act
            var response = await this.requestService.CreateRawMaterialRequest("otherUser", request);

            // assert
            ClassicAssert.AreEqual(false, response.Success);
        }

        /// <summary>
        /// Get raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetRawMaterialRequestByProductionOrderId_ExistingRequest_RequestResult()
        {
            // act
            var response = await this.requestService.GetRawMaterialRequestByProductionOrderId(4);

            // assert
            var request = (RawMaterialRequestModel)response.Response;
            ClassicAssert.IsTrue(response.Success);

            ClassicAssert.AreEqual(4, request.ProductionOrderIds[0]);
            ClassicAssert.AreEqual(1, request.OrderedProducts.Count);
        }

        /// <summary>
        /// Get raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetRawMaterialRequestByProductionOrderId_ExistingRequest_NullResultContent()
        {
            // act
            var response = await this.requestService.GetRawMaterialRequestByProductionOrderId(2000);

            // assert
            ClassicAssert.IsTrue(response.Success);
            ClassicAssert.IsNull(response.Response);
        }

        /// <summary>
        /// Get raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetRawMaterialPreRequest()
        {
            // arrange
            var productionOrderIds = new List<int> { 1, 2, 3 };
            var salesOrders = new List<int>();

            // act
            var response = await this.requestService.GetRawMaterialPreRequest(salesOrders, productionOrderIds);

            // assert
            ClassicAssert.IsTrue(response.Success);
            ClassicAssert.NotNull(response.Response);

            var preRequest = (RawMaterialRequestModel)response.Response;
            ClassicAssert.AreEqual(2, preRequest.ProductionOrderIds.Count);
            ClassicAssert.IsTrue(!preRequest.ProductionOrderIds.Contains(3));
            ClassicAssert.AreEqual(3, preRequest.OrderedProducts.Count);
            ClassicAssert.IsTrue(!preRequest.OrderedProducts.Any(x => x.ProductId.Equals("2")));
        }

        /// <summary>
        /// Check response results.
        /// </summary>
        /// <param name="result">Result.</param>
        /// <param name="success">Expected success.</param>
        /// <param name="numberOfSucceess">Expected success results.</param>
        /// <param name="numberOfFails">Expected fail results.</param>
        private void CheckAction(ResultModel result, bool success, int numberOfSucceess, int numberOfFails)
        {
            var content = (SuccessFailResults<object>)result.Response;
            ClassicAssert.AreEqual(success, result.Success);
            ClassicAssert.AreEqual(numberOfFails, content.Failed.Count);
            ClassicAssert.AreEqual(numberOfSucceess, content.Success.Count);
        }
    }
}
