// <summary>
// <copyright file="RequestFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Test.Facade.Request
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class RequestFacadeTest : BaseTest
    {
        private RequestFacade requestFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var resultDto = AutoFixtureProvider.Create<ResultModel>();
            resultDto.Success = true;

            var mockRequestService = new Mock<IRequestService>();
            mockRequestService.SetReturnsDefault(Task.FromResult(resultDto));

            this.requestFacade = new RequestFacade(mockRequestService.Object, mapper);
        }

        /// <summary>
        /// Test facade Map result.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest()
        {
            var requests = AutoFixtureProvider.Create<RawMaterialRequestDto>();
            requests.Signature = File.ReadAllText("SignatureBase64.txt");
            requests.OrderedProducts.ForEach(x => x.RequestQuantity = 1m);

            var response = await this.requestFacade.CreateRawMaterialRequest("userId", requests);

            ClassicAssert.IsNotNull(response);
            ClassicAssert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade Map result.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateRawMaterialRequest()
        {
            // arrange
            var requests = AutoFixtureProvider.Create<RawMaterialRequestDto>();
            requests.Signature = File.ReadAllText("SignatureBase64.txt");
            requests.OrderedProducts.ForEach(x => x.RequestQuantity = 1m);

            // act
            var response = await this.requestFacade.UpdateRawMaterialRequest("userId", requests);

            // arrange
            ClassicAssert.IsNotNull(response);
            ClassicAssert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade Map result.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetRawMaterialRequest()
        {
            // act
            var response = await this.requestFacade.GetRawMaterialRequest(1);

            // arrange
            ClassicAssert.IsNotNull(response);
            ClassicAssert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade Map result.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetRawMaterialPreRequest()
        {
            // act
            var response = await this.requestFacade.GetRawMaterialPreRequest(new List<int>(), new List<int>());

            // arrange
            ClassicAssert.IsNotNull(response);
            ClassicAssert.IsTrue(response.Success);
        }
    }
}
