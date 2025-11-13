// <summary>
// <copyright file="CatalogsInvoiceFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Facade
{
    /// <summary>
    /// Class CatalogsInvoiceFacadeTest.
    /// </summary>
    [TestFixture]
    public class CatalogsInvoiceFacadeTest : BaseTest
    {
        private CatalogsInvoiceFacade catalogsInvoiceFacade;

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
            var catalogsInvoiceService = new Mock<ICatalogsInvoiceService>();
            var resultDto = new ResultDto()
            {
                Code = 200,
                Success = true,
                Response = "Catálogo de errores procesado correctamente",
            };

            catalogsInvoiceService.SetReturnsDefault(Task.FromResult(resultDto));
            this.catalogsInvoiceFacade = new CatalogsInvoiceFacade(catalogsInvoiceService.Object);
        }

        /// <summary>
        /// InvoiceErrorsFromExcel.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceErrorsFromExcel()
        {
            // Act
            var response = await this.catalogsInvoiceFacade.InvoiceErrorsFromExcel();

            // Assert
            AssertResponse(response);
        }
    }
}