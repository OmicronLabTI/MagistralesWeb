// <summary>
// <copyright file="InvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services
{
    /// <summary>
    /// Class ProjectServiceTest.
    /// </summary>
    [TestFixture]
    public class InvoiceServiceTest : BaseTest
    {
        private IInvoiceService userService;
        private IInvoiceDao usersDao;
        private IMapper mapper;

        private DatabaseContext context;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalOrderValidationDBTest")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.SaveChanges();

            this.usersDao = new InvoiceDao(this.context);
            this.userService = new InvoiceService(this.mapper, this.usersDao);
        }
    }
}
