// <summary>
// <copyright file="UserFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Facade
{
    /// <summary>
    /// Class ProjectFacadeTest.
    /// </summary>
    [TestFixture]
    public class UserFacadeTest : BaseTest
    {
        private IInvoiceFacade invoiceFacade;
        private IInvoiceService projectService;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ProjectFacadeDB")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var response = new ResultDto
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockProject = new Mock<IInvoiceService>();
            this.projectService = mockProject.Object;
            mockProject.SetReturnsDefault(Task.FromResult(response));

            this.invoiceFacade = new InvoiceFacade(this.projectService);
        }

        /// <summary>
        /// ValidateConstructorInvalids.
        /// </summary>
        [Test]
        public void ValidateConstructorInvalids()
        {
            Assert.Throws<ArgumentNullException>(() => new InvoiceFacade(null));
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task CreateInvoice()
        {
            // arrange
            var model = new CreateInvoiceDto();

            // Act
            var response = await this.invoiceFacade.CreateInvoice(model);

            // Assert
            ClassicAssert.IsNotNull(response);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task GetInvoicesByRemissionId()
        {
            // arrange
            var model = new List<int>();

            // Act
            var response = await this.invoiceFacade.GetInvoicesByRemissionId(model);

            // Assert
            ClassicAssert.IsNotNull(response);
        }
    }
}
