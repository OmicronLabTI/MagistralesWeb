// <summary>
// <copyright file="InvoiceRetryFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.Invoice.Facade.InvoiceRetry.Impl;
using Omicron.Invoice.Services.InvoiceRetry;

namespace Omicron.Invoice.Test.Facade
{
    /// <summary>
    /// Class InvoiceRetryFacadeTest.
    /// </summary>
    [TestFixture]
    public class InvoiceRetryFacadeTest : BaseTest
    {
        private InvoiceRetryFacade invoiceRetryFacade;

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        public static void AssertResponse(ResultDto response)
        {
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var invoiceRetryService = new Mock<IInvoiceRetryService>();

            var resultDto = new ResultDto()
            {
                Code = 200,
                Success = true,
                Response = "response",
            };

            invoiceRetryService.SetReturnsDefault(Task.FromResult(resultDto));
            this.invoiceRetryFacade = new InvoiceRetryFacade(invoiceRetryService.Object);
        }

        /// <summary>
        /// GetDataToRetryCreateInvoicesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetDataToRetryCreateInvoicesAsync()
        {
            // Act
            var response = await this.invoiceRetryFacade.GetDataToRetryCreateInvoicesAsync();

            // Assert
            AssertResponse(response);
        }

        /// <summary>
        /// RetryCreateInvoicesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task RetryCreateInvoicesAsync()
        {
            // Act
            var response = await this.invoiceRetryFacade.RetryCreateInvoicesAsync(new InvoiceRetryRequestDto(), "executiontype");

            // Assert
            AssertResponse(response);
        }
    }
}
