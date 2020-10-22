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
    using Omicron.Usuarios.Services.Constants;
    using Omicron.Usuarios.Services.Mapping;
    using Omicron.Usuarios.Services.Pedidos;
    using Omicron.Usuarios.Services.SapAdapter;
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
            Assert.IsNotNull(response);
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
            var listIds = new List<string> { "6" };

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
            user.UserName = "userName1";
            user.Piezas = 10;

            // act
            var response = await this.userServices.UpdateUser(user);

            // assert
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(data.CountTotalFabOrders);
            Assert.IsNotNull(data.CountTotalOrders);
            Assert.IsNotNull(data.CountTotalPieces);
            Assert.IsNotNull(data.UserId);
            Assert.IsNotNull(data.UserName);
        }
    }
}
