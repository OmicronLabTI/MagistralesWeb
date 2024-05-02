// <summary>
// <copyright file="InventoryTransferRequestFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Facade
{
    /// <summary>
    /// Class InventoryTransferRequestFacadeTest.
    /// </summary>
    [TestFixture]
    public class InventoryTransferRequestFacadeTest
    {
        private IMapper mapper;

        private InventoryTransferRequestFacade inventoryTransferRequestFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();
            var mockInventoryTransferRequestService = new Mock<IInventoryTransferRequestService>();

            var resultDto = new ResultModel()
            {
                Code = 200,
                Success = true,
                Response = "response",
            };

            mockInventoryTransferRequestService.SetReturnsDefault(Task.FromResult(resultDto));
            this.inventoryTransferRequestFacade = new InventoryTransferRequestFacade(this.mapper, mockInventoryTransferRequestService.Object);
        }

        /// <summary>
        /// Test for create transfer request facade.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateTransferRequest()
        {
            // Arrange
            var transferRequestHeader = new List<TransferRequestHeaderDto>();

            // Act
            var response = await this.inventoryTransferRequestFacade.CreateTransferRequest(transferRequestHeader);

            // Assert
            AssertResponse(response);
        }

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        private static void AssertResponse(ResultDto response)
        {
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }
    }
}
