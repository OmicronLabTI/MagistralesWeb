// <summary>
// <copyright file="InvoiceFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Facade.Invoice
{
    /// <summary>
    /// Unit tests for <see cref="InvoiceFacade"/>.
    /// </summary>
    [TestFixture]
    public class InvoiceFacadeTest : BaseTest
    {
        private Mock<IInvoiceService> invoiceServiceMock;
        private IInvoiceFacade invoiceFacade;

        /// <summary>
        /// Initializes configuration before each test.
        /// </summary>
        [SetUp]
        public void Init()
        {
            this.invoiceServiceMock = new Mock<IInvoiceService>();
            this.invoiceFacade = new InvoiceFacade(this.invoiceServiceMock.Object);
        }

        /// <summary>
        /// Verifies that GetAutoBilling successfully delegates to the service layer and returns a valid result.
        /// </summary>
        [Test]
        public async Task GetAutoBilling_Success()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" }
            };

            var expectedResult = new ResultDto
            {
                Success = true,
                Code = 200,
                Response = new List<InvoiceModel>
                {
                    new InvoiceModel { Id = "INV-001", AlmacenUser = "USR-001" }
                }
            };

            this.invoiceServiceMock
                .Setup(x => x.GetAutoBillingAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await this.invoiceFacade.GetAutoBilling(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
            });

            this.invoiceServiceMock.Verify(x => x.GetAutoBillingAsync(parameters), Times.Once);
        }

        /// <summary>
        /// Verifies that GetAutoBilling correctly propagates an error result from the service layer.
        /// </summary>
        [Test]
        public async Task GetAutoBilling_Failure()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" }
            };

            var expectedResult = new ResultDto
            {
                Success = false,
                Code = 500,
                UserError = "Internal server error"
            };

            this.invoiceServiceMock
                .Setup(x => x.GetAutoBillingAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await this.invoiceFacade.GetAutoBilling(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(500));
                Assert.That(result.UserError, Is.EqualTo("Internal server error"));
            });

            this.invoiceServiceMock.Verify(x => x.GetAutoBillingAsync(parameters), Times.Once);
        }

        /// <summary>
        /// Verifies that GetAutoBilling properly propagates exceptions thrown by the service layer.
        /// </summary>
        [Test]
        public void GetAutoBilling_ThrowsException()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" }
            };

            this.invoiceServiceMock
                .Setup(x => x.GetAutoBillingAsync(It.IsAny<Dictionary<string, string>>()))
                .ThrowsAsync(new Exception("Service failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await this.invoiceFacade.GetAutoBilling(parameters));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Service failure"));

            this.invoiceServiceMock.Verify(x => x.GetAutoBillingAsync(parameters), Times.Once);
        }
    }
}
