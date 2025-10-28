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
            this.context.Users.AddRange(this.GetAllUserModel());
            this.context.SaveChanges();

            this.usersDao = new InvoiceDao(this.context);
            this.userService = new InvoiceService(this.mapper, this.usersDao);
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateGetAllAsync()
        {
            var response = await this.userService.GetAllAsync();

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any());
            Assert.That(response.Count().Equals(9));
        }
    }
}
