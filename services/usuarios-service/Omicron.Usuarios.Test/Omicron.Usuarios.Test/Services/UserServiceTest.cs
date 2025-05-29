// <summary>
// <copyright file="UserServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test.Services.Catalogs
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class UserServiceTest : BaseTest
    {
        private IUsersService userServices;

        private IMapper mapper;

        private IUserDao userDao;

        private DatabaseContext context;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [SetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var dbname = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(dbname)
                .Options;

            this.context = new DatabaseContext(options);
            this.context.Usuarios.AddRange(this.GetAllUsers());
            this.context.RoleModel.AddRange(this.GetRoles());
            this.context.SaveChanges();

            var mockPedidoService = new Mock<IPedidosService>();
            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultFabOrders()));

            this.userDao = new UserDao(this.context);
            this.userServices = new UsersService(this.mapper, this.userDao, mockPedidoService.Object, mockSapAdapter.Object);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateGetAllUsers()
        {
            var result = await this.userServices.GetAllUsersAsync();

            Assert.That(result != null);
            Assert.That(result.Any());
        }

        /// <summary>
        /// Method to validate get user by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateSpecificUsers()
        {
            var result = await this.userServices.GetUserAsync(2);

            Assert.That(result == null);
        }

        /// <summary>
        /// test the insert.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InsertUser()
        {
            // Arrange
            var user = this.GetUserDto();
            user.Id = "100";

            // Act
            var result = await this.userServices.InsertUser(user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result);
        }

        /// <summary>
        /// Test To create user.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateUser()
        {
            // arrange
            var user = this.GetUserModel();
            user.UserName = "ABC";

            // act
            var response = await this.userServices.CreateUser(user);

            // arrange
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// Test To create user.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateUserAnyClassification()
        {
            // arrange
            var user = new UserModel
            {
                UserName = "pruebaDesinger",
                FirstName = "prueba",
                LastName = "usuario",
                Password = "QXhpdHkyMDI0",
                Activo = 1,
                Role = 4,
                Asignable = 1,
                Piezas = 200,
                Classification = "MN, MG",
                TechnicalRequire = false,
                TecnicId = null,
            };

            // act
            var response = await this.userServices.CreateUser(user);

            // arrange
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// Test To create user.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateUserAllClassification()
        {
            // arrange
            var user = new UserModel
            {
                UserName = "pruebaLogistica",
                FirstName = "prueba",
                LastName = "usuario",
                Password = "QXhpdHkyMDI0",
                Activo = 1,
                Role = 3,
                Asignable = 1,
                Piezas = 200,
                Classification = "Todas",
                TechnicalRequire = false,
                TecnicId = null,
            };

            // act
            var response = await this.userServices.CreateUser(user);

            // arrange
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// creates the user with error the user exist.
        /// </summary>
        [Test]
        public void CreateUserErrorByRepeatedUsername()
        {
            // arrange
            var user = this.GetUserModel();

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await this.userServices.CreateUser(user));
        }

        /// <summary>
        /// Gets the users with offset.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetAllUsersWithOffsetLimit()
        {
            // arrange
            var dic = new Dictionary<string, string>();
            dic.Add("offset", "2");
            dic.Add("limit", "10");

            // act
            var response = await this.userServices.GetUsers(dic);

            // Assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Gets the users with offset.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetAllUsersWithFilters()
        {
            // arrange
            var dic = new Dictionary<string, string>();
            dic.Add("offset", "2");
            dic.Add("limit", "10");
            dic.Add(ServiceConstants.UserName, "ale");
            dic.Add(ServiceConstants.FirstName, "alej");
            dic.Add(ServiceConstants.LastName, "OJEDA");
            dic.Add(ServiceConstants.Role, "1");
            dic.Add(ServiceConstants.Assignable, "1");
            dic.Add(ServiceConstants.Status, "1");

            // act
            var response = await this.userServices.GetUsers(dic);

            // Assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Gets the users with offset.
        /// </summary>
        /// <param name="classifications">Clasiffications.</param>
        /// <returns>returns nothing.</returns>
        [Test]
        [TestCase("MN")]
        public async Task GetAllUsersWithFiltersClassifications(string classifications)
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { ServiceConstants.TypeQfb, classifications },
            };

            // act
            var response = await this.userServices.GetUsers(dic);
            var result = response.Response as List<UserModel>;

            TestContext.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "Janettelog"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "danyTodas"));
        }

        /// <summary>
        /// Gets the users with offset.
        /// </summary>
        /// <param name="classifications">Clasiffications.</param>
        /// <returns>returns nothing.</returns>
        [Test]
        [TestCase("MN, BE")]
        public async Task GetAllUsersWithFiltersClassificationsTwo(string classifications)
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { ServiceConstants.TypeQfb, classifications },
            };

            // act
            var response = await this.userServices.GetUsers(dic);
            var result = response.Response as List<UserModel>;

            TestContext.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "Janettelog"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "danyTodas"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "vicDesigner"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "benny"));
        }

        /// <summary>
        /// Gets the users with offset.
        /// </summary>
        /// <param name="classifications">Clasiffications.</param>
        /// <returns>returns nothing.</returns>
        [Test]
        [TestCase("MG, BE, DZ")]
        public async Task GetAllUsersWithFiltersClassificationsThree(string classifications)
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { ServiceConstants.TypeQfb, classifications },
            };

            // act
            var response = await this.userServices.GetUsers(dic);
            var result = response.Response as List<UserModel>;

            TestContext.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "vicDesigner"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "KarenAD"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "danyTodas"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "javierlog"));
            Assert.That(result, Has.Some.Matches<UserModel>(u => u.UserName == "benny"));
        }

        /// <summary>
        /// Test to  delete user.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task DeleteUser()
        {
            // arrange
            var listIds = new List<string> { "6" };

            // act
            var response = await this.userServices.DeleteUser(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task UpdateUser()
        {
            // arrange
            var user = this.GetUserModel();
            user.Id = "1";
            user.UserName = "userName1";
            user.Piezas = 10;

            // act
            var response = await this.userServices.UpdateUser(user);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task UpdateTechnicalUser()
        {
            // arrange
            var user = new UserModel { Id = "11", FirstName = "TecnicoPrueba Update", LastName = "TecnicoPrueba Update Apellido", UserName = "TecnicoPrueba Update", Password = "QXhpdHkyMDIw", Role = 9, Activo = 1, Piezas = 200, Asignable = 0, Deleted = false, };

            // act
            var response = await this.userServices.UpdateUser(user);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user technical Role.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task UpdateTechnicalUserRole()
        {
            // arrange
            var user = new UserModel { Id = "13", FirstName = "TecnicoPrueba Update Role", LastName = "TecnicoPrueba Update Apellido Role", UserName = "TecnicoPruebaUpdateRole", Password = "QXhpdHkyMDIw", Role = 2, Activo = 1, Piezas = 200, Asignable = 1, Deleted = false, };

            // act
            var response = await this.userServices.UpdateUser(user);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        [Test]
        public void UpdateUserUserNotExist()
        {
            // arrange
            var user = this.GetUserModel();

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await this.userServices.UpdateUser(user));
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task GetUser()
        {
            // arrange
            var user = "George";

            // act
            var response = await this.userServices.GetUser(user);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task GetQfb()
        {
            var roleId = "1";

            // act
            var response = await this.userServices.GetUsersByRole(roleId);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task GetActiveQfb()
        {
            // act
            var response = await this.userServices.GetActiveQfbWithOrcerCount();

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task GetUsersById()
        {
            // arrange
            var listIds = new List<string> { "1" };

            // act
            var response = await this.userServices.GetUsersById(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        [Test]
        public void UserWithOrderCountModelTest()
        {
            // arrange
            var data = new UserWithOrderCountModel
            {
                CountTotalFabOrders = 10,
                CountTotalOrders = 10,
                CountTotalPieces = 10,
                UserId = "asd",
                UserName = "asd",
            };

            // assert
            Assert.That(data.CountTotalFabOrders, Is.Not.Null);
            Assert.That(data.CountTotalOrders, Is.Not.Null);
            Assert.That(data.CountTotalPieces, Is.Not.Null);
            Assert.That(data.UserId, Is.Not.Null);
            Assert.That(data.UserName, Is.Not.Null);
        }

        /// <summary>
        /// test to get all tecnic users.
        /// </summary>
        /// <returns>returns list of users.</returns>
        [Test]
        public async Task GetAllUsersTecnics()
        {
            // act
            var response = await this.userServices.GetUsersTecnic();

            // Assert
            Assert.That(response.Response, Is.Not.Null);
        }

        /// <summary>
        /// test to get all tecnic users.
        /// </summary>
        /// <param name="id">Id User to get relation with tecnic.</param>
        /// <returns>returns list of users.</returns>
        [Test]
        [TestCase("9")]
        [TestCase("8")]
        [TestCase("Noexiste")]
        [TestCase("10")]
        public async Task GetRelationalUserInfor(string id)
        {
            // act
            var response = await this.userServices.GetRelationalUserInfor(id);

            // Assert
            if (id.Equals("Noexiste") || id.Equals("10"))
            {
                Assert.That(response.Comments, Is.Null);
            }
            else
            {
                Assert.That(response.Comments, Is.Not.Null);
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]

        public async Task GetQfbInfoByIds()
        {
            var qfbIds = new List<string>
            {
                "6bc7f8a8-8617-43ac-a804-79cf9667b801",
                "6bc7f8a8-8617-43ac-a804-79cf9667b802",
                "6bc7f8a8-8617-43ac-a804-79cf9667b803",
                "6bc7f8a8-8617-43ac-a804-79cf9667b804",
                "6bc7f8a8-8617-43ac-a804-79cf9667b807",
            };

            // act
            var response = await this.userServices.GetQfbInfoByIds(qfbIds);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(200.Equals(response.Code));
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);
        }
    }
}
