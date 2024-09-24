// <summary>
// <copyright file="CatalogServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Services
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class CatalogServiceTest : BaseTest
    {
        private ICatalogService catalogService;

        private IMapper mapper;

        private ICatalogDao catalogDao;

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
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.RoleModel.AddRange(this.GetListRoles());
            this.context.ParametersModel.AddRange(this.GetParameters());
            this.context.ClassificationQfbModel.AddRange(this.GetActiveClassificationQfbModel());
            this.context.SaveChanges();

            this.catalogDao = new CatalogDao(this.context);
            this.catalogService = new CatalogService(this.catalogDao);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAllRolesTest()
        {
            var result = await this.catalogService.GetRoles();

            ClassicAssert.True(result != null);
            ClassicAssert.IsNotNull(result.Response);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetParams()
        {
            var dictValues = new Dictionary<string, string>
            {
                { "Email", "Email" },
            };

            var result = await this.catalogService.GetParamsContains(dictValues);

            ClassicAssert.True(result != null);
            ClassicAssert.IsNotNull(result.Response);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActiveClassificationQfb()
        {
            var result = await this.catalogService.GetActiveClassificationQfb();

            // assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsTrue(result.Code == 200);
            ClassicAssert.IsNotNull(result.Response);
            ClassicAssert.IsNull(result.UserError);
            ClassicAssert.IsInstanceOf<object>(result.Response);
        }
    }
}
