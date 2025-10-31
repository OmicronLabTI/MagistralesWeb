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
        private IInvoiceFacade projectFacade;
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

            var mockProject = new Mock<IInvoiceService>();
            this.projectService = mockProject.Object;
            this.projectFacade = new InvoiceFacade(this.projectService);
        }

        /// <summary>
        /// ValidateConstructorInvalids.
        /// </summary>
        [Test]
        public void ValidateConstructorInvalids()
        {
            Assert.Throws<ArgumentNullException>(() => new InvoiceFacade(null));
        }
    }
}
