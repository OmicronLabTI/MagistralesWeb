// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Facade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.Catalogos.Dtos.User;
    using Omicron.Catalogos.Entities.Model;
    using Omicron.Catalogos.Facade.Catalogs;
    using Omicron.Catalogos.Facade.Catalogs.Users;
    using Omicron.Catalogos.Services.Catalogs;
    using Omicron.Catalogos.Services.Mapping;
    using Omicron.Catalogos.Services.User;

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
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }
    }
}
