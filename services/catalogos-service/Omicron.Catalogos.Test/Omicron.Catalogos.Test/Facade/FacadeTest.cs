// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Facade
{
    /// <summary>
    /// class for test.
    /// </summary>
    [TestFixture]
    public class FacadeTest : BaseTest
    {
        private UserFacade userFacade;

        private CatalogFacade catalogFacade;

        private IMapper mapper;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var mockServices = new Mock<IUsersService>();
            var user = this.GetUserDto();
            IEnumerable<UserDto> listUser = new List<UserDto> { user };

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
            };

            mockServices
                .Setup(m => m.GetAllUsersAsync())
                .Returns(Task.FromResult(listUser));

            mockServices
                .Setup(m => m.GetUserAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(user));

            mockServices
                .Setup(m => m.InsertUser(It.IsAny<UserDto>()))
                .Returns(Task.FromResult(true));

            var mockServicesCat = new Mock<ICatalogService>();
            mockServicesCat
                .Setup(m => m.GetRoles())
                .Returns(Task.FromResult(response));

            mockServicesCat
                .Setup(m => m.GetParamsContains(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockServicesCat
               .Setup(m => m.GetActiveClassificationQfb())
               .Returns(Task.FromResult(response));

            mockServicesCat
               .Setup(m => m.UploadWarehouseFromExcel())
               .Returns(Task.FromResult(response));

            mockServicesCat
                .Setup(m => m.UploadProductTypeColorsFromExcel())
                .Returns(Task.FromResult(response));

            mockServicesCat
               .Setup(m => m.UploadConfigurationRouteFromExcel())
               .Returns(Task.FromResult(response));

            mockServicesCat
               .Setup(m => m.GetActiveRouteConfigurationsForProducts())
               .Returns(Task.FromResult(response));

            mockServicesCat
               .Setup(m => m.GetProductsColors(It.IsAny<List<string>>()))
               .Returns(Task.FromResult(response));

            this.catalogFacade = new CatalogFacade(mockServicesCat.Object, this.mapper);
            this.userFacade = new UserFacade(mockServices.Object);
        }

        /// <summary>
        /// Test for selecting all users.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task GetAllUsersAsyncTest()
        {
            // arrange

            // Act
            var response = await this.userFacade.GetListUsersActive();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any(), Is.True);
        }

        /// <summary>
        /// gets the user.
        /// </summary>
        /// <returns>the user with the correct id.</returns>
        [Test]
        public async Task GetListUserActive()
        {
            // arrange
            var id = 10;

            // Act
            var response = await this.userFacade.GetListUserActive(id);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Id, Is.EqualTo(id));
        }

        /// <summary>
        /// Test for inseting users.
        /// </summary>
        /// <returns>the bool if it was inserted.</returns>
        [Test]
        public async Task InsertUser()
        {
            // Arrange
            var user = new UserDto();

            // Act
            var response = await this.userFacade.InsertUser(user);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.True);
        }

        /// <summary>
        /// Test getting the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        [Test]
        public async Task GetAllRoles()
        {
            // Arrange
            // Act
            var response = await this.catalogFacade.GetRoles();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// Test getting the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        [Test]
        public async Task GetParams()
        {
            // Arrange
            var containsValue = new Dictionary<string, string>();

            // Act
            var response = await this.catalogFacade.GetParamsContains(containsValue);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// Test getting the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        [Test]
        public async Task GetActiveClassificationQfb()
        {
            // Act
            var response = await this.catalogFacade.GetActiveClassificationQfb();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// Test getting the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        [Test]
        public async Task UploadWarehouseFromExcel()
        {
            // Act
            var response = await this.catalogFacade.UploadWarehouseFromExcel();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// Test to verify upload product type colors from excel.
        /// </summary>
        /// <returns>Upload product type colors from excel result.</returns>
        [Test]
        public async Task UploadProductTypeColorsFromExcel()
        {
            // Act
            var response = await this.catalogFacade.UploadProductTypeColorsFromExcel();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// Test to verify upload product type colors from excel.
        /// </summary>
        /// <returns>Upload product type colors from excel result.</returns>
        [Test]
        public async Task GetProductsColors()
        {
            // Act
            var response = await this.catalogFacade.GetProductsColors(new List<string>());

            // Assert
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// Test to verify upload sorting route from excel.
        /// </summary>
        /// <returns> upload sorting route from excel. </returns>
        [Test]
        public async Task UploadConfigRouteFromExcel()
        {
            // Act
            var response = await this.catalogFacade.UploadConfigRouteFromExcel();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }

        /// <summary>
        /// GetActiveRouteConfigurationsForProducts.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        [Test]
        public async Task GetActiveRouteConfigurationsForProducts()
        {
            // Act
            var response = await this.catalogFacade.GetActiveRouteConfigurationsForProducts();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
        }
    }
}
