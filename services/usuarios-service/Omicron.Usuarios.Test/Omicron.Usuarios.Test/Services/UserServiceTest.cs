// <summary>
// <copyright file="UserServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test.Services.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Usuarios.DataAccess.DAO.User;
    using Omicron.Usuarios.Entities.Context;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Services.Mapping;
    using Omicron.Usuarios.Services.User;

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
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.Usuarios.AddRange(this.GetAllUsers());
            this.context.SaveChanges();

            this.userDao = new UserDao(this.context);
            this.userServices = new UsersService(this.mapper, this.userDao);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateGetAllUsers()
        {
            var result = await this.userServices.GetAllUsersAsync();

            Assert.True(result != null);
            Assert.True(result.Any());
        }

        /// <summary>
        /// Method to validate get user by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateSpecificUsers()
        {
            var result = await this.userServices.GetUserAsync(2);

            Assert.True(result == null);
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
            user.Id = "12";

            // Act
            var result = await this.userServices.InsertUser(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Validate credentials.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task ValidateCredentials()
        {
            // arrange
            var login = new LoginModel
            {
                Password = "QXhpdHkyMDIw",
                Username = "Benji",
            };

            // act
            var response = await this.userServices.ValidateCredentials(login);

            // Assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Validate credentials.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task ValidateCredentialsUserNotExist()
        {
            // arrange
            var login = new LoginModel
            {
                Password = "abcde",
                Username = "Gustavo",
            };

            // act
            var response = await this.userServices.ValidateCredentials(login);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
        }

        /// <summary>
        /// Validate credentials.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task ValidateCredentialsPasswordIncorrect()
        {
            // arrange
            var login = new LoginModel
            {
                Password = "abcde",
                Username = "Benji",
            };

            // act
            var response = await this.userServices.ValidateCredentials(login);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
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
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// creates the user with error the user exist.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CreateUserErrorByRepeatedUsername()
        {
            // arrange
            var user = this.GetUserModel();

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await this.userServices.CreateUser(user));
        }

        /// <summary>
        /// creates the user with error the user exist.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CreateUserErrorByDataBase()
        {
            // arrange
            var user = this.GetUserModel();
            user.UserName = "test";
            var mockUser = new Mock<IUserDao>();

            mockUser
                .Setup(x => x.GetUserByUserName(It.IsAny<string>()))
                .Returns(Task.FromResult<UserModel>(null));

            mockUser
                .Setup(x => x.InsertUser(It.IsAny<UserModel>()))
                .Returns(Task.FromResult(false));

            var userServiceMock = new UsersService(this.mapper, mockUser.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await userServiceMock.CreateUser(user));
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
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test to  delete user.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task DeleteUser()
        {
            // arrange
            var listIds = new List<string> { "1", };

            // act
            var response = await this.userServices.DeleteUser(listIds);

            // assert
            Assert.IsNotNull(response);
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

            // act
            var response = await this.userServices.UpdateUser(user);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <returns>the user.</returns>
        [Test]
        public async Task UpdateUserUserNotExist()
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
        }
    }
}
