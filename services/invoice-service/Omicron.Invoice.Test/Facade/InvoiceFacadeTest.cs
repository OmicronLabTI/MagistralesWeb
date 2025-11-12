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
    /// Class InvoiceFacadeTest.
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
        /// Test that verifies successful invocation of GetAutoBilling through the facade.
        /// Ensures the method delegates execution to the service layer and returns a valid ResultDto.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task GetAutoBilling_Success()
        {
            // arrange
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

            // act
            var result = await this.invoiceFacade.GetAutoBilling(parameters);

            // assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
            this.invoiceServiceMock.Verify(x => x.GetAutoBillingAsync(parameters), Times.Once);
        }

        /// <summary>
        /// Test that validates behavior when the service returns an error ResultDto.
        /// Ensures the facade properly propagates the failed result without alteration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task GetAutoBilling_Failure()
        {
            // arrange
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

            // act
            var result = await this.invoiceFacade.GetAutoBilling(parameters);

            // assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsFalse(result.Success);
            ClassicAssert.AreEqual(500, result.Code);
            ClassicAssert.AreEqual("Internal server error", result.UserError);
            this.invoiceServiceMock.Verify(x => x.GetAutoBillingAsync(parameters), Times.Once);
        }

        /// <summary>
        /// Test that validates exception handling when the service throws an error.
        /// Ensures the facade properly propagates or rethrows exceptions as expected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public void GetAutoBilling_ThrowsException()
        {
            // arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" }
            };

            this.invoiceServiceMock
                .Setup(x => x.GetAutoBillingAsync(It.IsAny<Dictionary<string, string>>()))
                .ThrowsAsync(new Exception("Service failure"));

            // act & assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await this.invoiceFacade.GetAutoBilling(parameters));
            Assert.That(ex.Message, Is.EqualTo("Service failure"));
            this.invoiceServiceMock.Verify(x => x.GetAutoBillingAsync(parameters), Times.Once);
        }
    }
}
