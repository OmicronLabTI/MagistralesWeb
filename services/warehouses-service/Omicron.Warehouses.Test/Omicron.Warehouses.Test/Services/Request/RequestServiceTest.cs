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
    using System.IO;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.Warehouses.DataAccess.DAO.Request;
    using Omicron.Warehouses.Entities.Context;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Clients;
    using Omicron.Warehouses.Services.Mapping;
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
            request[0].ProductionOrderId = 1;
            request[0].Signature = new byte[0];
            request[0].OrderedProducts = AutoFixtureProvider.CreateList<RawMaterialRequestDetailModel>(1);
            request[0].OrderedProducts[0].Id = 1;
            request[0].OrderedProducts[0].RequestId = 1;
            request[0].OrderedProducts[0].ProductId = "P001";
            request[0].OrderedProducts[0].Unit = "Kilogramo";

            this.context.RawMaterialRequests.AddRange(request);
            this.context.RawMaterialRequestDetails.AddRange(request[0].OrderedProducts);

            this.context.SaveChanges();
        }

        /// <summary>
        /// Create mock users.
        /// </summary>
        /// <returns>Mock users.</returns>
        public List<UserModel> GetMockUsers()
        {
            return AutoFixtureProvider.CreateList<UserModel>(1);
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
            this.mockUsersService.Setup(x => x.GetUsersById(It.IsAny<List<string>>())).Returns(Task.FromResult(this.GetMockUsers()));

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
            var request = AutoFixtureProvider.CreateList<RawMaterialRequestModel>(1);
            request[0].ProductionOrderId = 2;
            request[0].OrderedProducts = AutoFixtureProvider.CreateList<RawMaterialRequestDetailModel>(3);

            // act
            var response = await this.requestService.CreateRawMaterialRequest(this.userId, request);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0));
        }

        /// <summary>
        /// Create new raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest_RequestAlreadyExists_FailResults()
        {
            // arrange
            var request = AutoFixtureProvider.CreateList<RawMaterialRequestModel>(1);
            request[0].ProductionOrderId = 1;

            // act
            var response = await this.requestService.CreateRawMaterialRequest(this.userId, request);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1));
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
            Assert.AreEqual(1, request.ProductionOrderId);
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
        /// Update raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task UpdateRawMaterialRequest_ExistingRequest_SuccessResult()
        {
            // arrange
            ConverterBase64ToByteArray converter = new ConverterBase64ToByteArray();
            var existingItem = await this.requestDao.GetRawMaterialRequestByProductionOrderId(1);

            var request = JsonConvert.DeserializeObject<RawMaterialRequestModel>(JsonConvert.SerializeObject(existingItem));
            request.ProductionOrderId = 1;
            request.Signature = converter.Convert(File.ReadAllText("SignatureBase64.txt"), null);
            request.Observations = "New comments.";
            request.OrderedProducts[0].RequestQuantity = 199;
            request.OrderedProducts.Add(new RawMaterialRequestDetailModel
            {
                RequestQuantity = 1,
                ProductId = "P0002",
                Description = "Desc 002",
            });

            // act
            var response = await this.requestService.UpdateRawMaterialRequest(this.userId, new List<RawMaterialRequestModel> { request });

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0));
        }

        /// <summary>
        /// Update raw material request.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task UpdateRawMaterialRequest_NotExistingRequest_FailResult()
        {
            // arrange
            var request = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            request.ProductionOrderId = 99;

            // act
            var response = await this.requestService.UpdateRawMaterialRequest(this.userId, new List<RawMaterialRequestModel> { request });

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1));
        }

        /// <summary>
        /// Check response results.
        /// </summary>
        /// <param name="result">Result.</param>
        /// <param name="success">Expected success.</param>
        /// <param name="numberOfSucceess">Expected success results.</param>
        /// <param name="numberOfFails">Expected fail results.</param>
        /// <returns>Validation flag.</returns>
        private bool CheckAction(ResultModel result, bool success, int numberOfSucceess, int numberOfFails)
        {
            var content = (SuccessFailResults<RawMaterialRequestModel>)result.Response;

            return result.Success.Equals(success) &&
                    numberOfFails.Equals(content.Failed.Count) &&
                    numberOfSucceess.Equals(content.Success.Count);
        }
    }
}
