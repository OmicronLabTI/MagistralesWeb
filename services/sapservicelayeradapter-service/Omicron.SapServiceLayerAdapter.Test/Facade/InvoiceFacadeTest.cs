// <summary>
// <copyright file="InvoiceFacadeTest.cs" company="Axity">
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
    public class InvoiceFacadeTest
    {
        private InvoiceFacade invoiceFacade;

        private IMapper mapper;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();
            var mockInvoiceService = new Mock<IInvoiceService>();

            var resultDto = new ResultModel()
            {
                Code = 200,
                Success = true,
                Response = "response",
            };

            mockInvoiceService.SetReturnsDefault(Task.FromResult(resultDto));
            this.invoiceFacade = new InvoiceFacade(this.mapper, mockInvoiceService.Object);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task CloseSampleOrders()
        {
            // Arrange
            var invoiceId = 0;
            var packageInformationSend = new TrackingInformationDto();

            // Act
            var response = await this.invoiceFacade.UpdateInvoiceTrackingInfo(invoiceId, packageInformationSend);

            // Assert
            AssertResponse(response);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task CreateInvoiceByDeliveries()
        {
            // Act
            var response = await this.invoiceFacade.CreateInvoiceByDeliveries(new List<int>());

            // Assert
            AssertResponse(response);
        }

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        private static void AssertResponse(ResultDto response)
        {
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Code, Is.EqualTo(200));
        }
    }
}
