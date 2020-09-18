// <summary>
// <copyright file="RequestServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Test.Services.Request
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Omicron.Warehouses.DataAccess.DAO.Request;
    using Omicron.Warehouses.Entities.Context;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Clients;
    using Omicron.Warehouses.Services.Request;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class RequestServiceTest : BaseTest
    {
        private IRequestService requestService;
        private Mock<IUsersService> mockUsersService;
        private IRequestDao requestDao;
        private DatabaseContext context;
        private string userId = "abc";

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
            this.context.RawMaterialRequestOrders.AddRange(request[0].ProductionOrderIds.Select(x => new RawMaterialRequestOrderModel { Id = 1, ProductionOrderId = 1, RequestId = 1 }));

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
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.InitializeInMemoryDb();
            this.requestDao = new RequestDao(this.context);
            this.mockUsersService = new Mock<IUsersService>();
            this.mockUsersService.Setup(x => x.GetUsersById(It.IsAny<string[]>())).Returns(Task.FromResult(this.GetMockUsers()));

            this.requestService = new RequestService(this.requestDao, this.mockUsersService.Object);
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
        public async Task CreateRawMaterialRequest_RequestAlreadyExists_FailResults()
        {
            // arrange
            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.ProductionOrderIds = new List<int> { 1 };

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
            Assert.AreEqual(false, response.Success);
        }

        /// <summary>
        /// Get raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetRawMaterialRequestByProductionOrderId_ExistingRequest_RequestResult()
        {
            // act
            var response = await this.requestService.GetRawMaterialRequestByProductionOrderId(1);

            // assert
            var request = (RawMaterialRequestModel)response.Response;
            Assert.IsTrue(response.Success);

            Assert.AreEqual(1, request.ProductionOrderIds[0]);
            Assert.AreEqual(1, request.OrderedProducts.Count);
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
            Assert.IsTrue(response.Success);
            Assert.IsNull(response.Response);
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
            Assert.AreEqual(success, result.Success);
            Assert.AreEqual(numberOfFails, content.Failed.Count);
            Assert.AreEqual(numberOfSucceess, content.Success.Count);
        }
    }
}
