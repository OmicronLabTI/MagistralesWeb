// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test.Facade
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.Usuarios.Dtos.Models;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Facade.Catalogs.Users;
    using Omicron.Usuarios.Services.Mapping;
    using Omicron.Usuarios.Services.User;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class FacadeTest : BaseTest
    {
        private UserFacade userFacade;

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

            mockServices
                .Setup(m => m.GetAllUsersAsync())
                .Returns(Task.FromResult(listUser));

            mockServices
                .Setup(m => m.GetUserAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(user));

            mockServices
                .Setup(m => m.InsertUser(It.IsAny<UserDto>()))
                .Returns(Task.FromResult(true));

            var result = new ResultModel
            {
                Success = true,
                Code = 200,
            };

            mockServices
                .Setup(m => m.ValidateCredentials(It.IsAny<LoginModel>()))
                .Returns(Task.FromResult(result));

            mockServices
                .Setup(m => m.CreateUser(It.IsAny<UserModel>()))
                .Returns(Task.FromResult(result));

            mockServices
                .Setup(m => m.GetUsers(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(result));

            this.userFacade = new UserFacade(mockServices.Object, this.mapper);
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
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Any());
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
            Assert.IsNotNull(response);
            Assert.AreEqual(id.ToString(), response.Id);
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
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
        }

        /// <summary>
        /// Validate Credentials test.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task ValidateCredentialsTest()
        {
            // Arrange
            var user = new LoginDto
            {
                Password = "password",
                Username = "Gustavo",
            };

            // Act
            var response = await this.userFacade.ValidateCredentials(user);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test to create user.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CreateUserTest()
        {
            // arrange
            var user = this.GetUserDto();

            // Act
            var response = await this.userFacade.CreateUser(user);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// test to get all users.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetAllUsersWithOffset()
        {
            // arrange
            var dic = new Dictionary<string, string>();
            dic.Add("Offset", "1");
            dic.Add("Limit", "2");

            // act
            var response = await this.userFacade.GetUsers(dic);

            // Assert
            Assert.IsNotNull(response);
        }
    }
}
