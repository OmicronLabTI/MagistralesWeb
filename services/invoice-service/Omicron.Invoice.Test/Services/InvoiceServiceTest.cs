// <summary>
// <copyright file="InvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.Invoice.Services.Catalog;
using Omicron.Invoice.Services.Redis;
using Omicron.Invoice.Services.ServiceLayer;

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

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();

            this.usersDao = new InvoiceDao(this.context);
            this.userService = new InvoiceService(this.usersDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object);
        }
    }
}
