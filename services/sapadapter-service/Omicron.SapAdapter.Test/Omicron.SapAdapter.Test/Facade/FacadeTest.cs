// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.Dtos.User;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Facade.Catalogs.Users;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Services.Mapping;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class FacadeTest : BaseTest
    {
        private UserFacade userFacade;

        private SapFacade sapFacade;

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

            var mockSapServices = new Mock<ISapService>();

            var response = new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = true,
                Success = true,
                UserError = string.Empty,
            };

            mockSapServices
                .Setup(m => m.GetOrders(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetOrderDetails(It.IsAny<int>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetPedidoWithDetail(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            this.sapFacade = new SapFacade(mockSapServices.Object, this.mapper);
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
            Assert.AreEqual(id, response.Id);
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
        /// Test to get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        [Test]
        public async Task GetPedidos()
        {
            // Arrange
            var dict = new Dictionary<string, string>();

            // Act
            var response = this.sapFacade.GetOrders(dict);

            // Assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Get detalle de pedido.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetDetallePedidos()
        {
            // Arrange
            var docEntry = "10";

            // act
            var response = await this.sapFacade.GetDetallePedidos(docEntry);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetPedidoWithDetail()
        {
            // arrange
            var listDocs = new List<int> { 1, 2, 3 };

            // act
            var response = await this.sapFacade.GetPedidoWithDetail(listDocs);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }
    }
}
