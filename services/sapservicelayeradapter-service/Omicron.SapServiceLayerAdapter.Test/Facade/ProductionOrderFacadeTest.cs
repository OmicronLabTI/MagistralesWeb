// <summary>
// <copyright file="ProductionOrderFacadeTest.cs" company="Axity">
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
    public class ProductionOrderFacadeTest
    {
        private ProductionOrderFacade productionOrderfacade;
        private IMapper mapper;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();
            var mockService = new Mock<IProductionOrderService>();

            var resultDto = new ResultModel()
            {
                Code = 200,
                Success = true,
                Response = "response",
            };

            mockService.SetReturnsDefault(Task.FromResult(resultDto));
            this.productionOrderfacade = new ProductionOrderFacade(this.mapper, mockService.Object);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task FinishOrder()
        {
            // Act
            var response = await this.productionOrderfacade.FinishOrder(new List<CloseProductionOrderDto>());

            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// Test for update formula.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task UpdateFormula()
        {
            // Act
            var response = await this.productionOrderfacade.UpdateFormula(new UpdateFormulaDto());

            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// Test for Create FabOrder.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task CreateFabOrder()
        {
            // Act
            var response = await this.productionOrderfacade.CreateFabOrder(new List<OrderWithDetailDto>());

            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// Test for Create FabOrder.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task UpdateFabOrders()
        {
            // Act
            var response = await this.productionOrderfacade.UpdateFabOrders(new List<UpdateFabOrderDto>());

            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// Test for Create FabOrder.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task UpdateProductionOrdersBatches()
        {
            // Act
            var response = await this.productionOrderfacade.UpdateProductionOrdersBatches(new List<AssignBatchDto>());

            // Assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Test for Cancel FabOrder.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task CancelFabOrders()
        {
            // Act
            var response = await this.productionOrderfacade.CancelProductionOrder(new CancelOrderDto());

            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
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