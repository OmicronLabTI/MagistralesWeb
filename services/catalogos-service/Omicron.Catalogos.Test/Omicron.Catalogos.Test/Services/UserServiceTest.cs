// <summary>
// <copyright file="UserServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Services.Catalogs
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Catalogos.DataAccess.DAO.User;
    using Omicron.Catalogos.Entities.Context;
    using Omicron.Catalogos.Services.Mapping;
    using Omicron.Catalogos.Services.User;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;

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
            this.context.CatUser.AddRange(this.GetAllUsers());
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

            Assert.True(result != null);
            Assert.True(result.FirstName == "Jorge");
        }
    }
}
