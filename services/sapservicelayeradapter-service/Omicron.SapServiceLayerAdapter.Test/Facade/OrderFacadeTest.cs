// <summary>
// <copyright file="OrdersFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Facade
{
    /// <summary>
    /// Class ProductValidationsFacadeTest.
    /// </summary>
    [TestFixture]
    public class OrderFacadeTest
    {
        private OrderFacade ordersFacade;

        private IMapper mapper;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();
            var mockOrdersService = new Mock<IOrderService>();

            var resultDto = new ResultModel()
            {
                Code = 200,
                Success = true,
                Response = "response",
            };

            mockOrdersService.SetReturnsDefault(Task.FromResult(resultDto));
            this.ordersFacade = new OrderFacade(this.mapper, mockOrdersService.Object);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task GetLastGeneratedOrder()
        {
            // Act
            var response = await this.ordersFacade.GetLastGeneratedOrder();

            // Assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        public void AssertResponse(ResultDto response)
        {
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }
    }
}
