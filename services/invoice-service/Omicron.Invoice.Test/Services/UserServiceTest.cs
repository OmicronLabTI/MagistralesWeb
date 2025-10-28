// <summary>
// <copyright file="UserServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Test.Services
{
    using Omicron.Invoice.Common.DTOs.Requests.Users;
    using Omicron.Invoice.Persistence.DAO.Users;
    using Omicron.Invoice.Persistence.DAO.Users.Impl;
    using Omicron.Invoice.Services.Mapping;
    using Omicron.Invoice.Services.Users;
    using Omicron.Invoice.Services.Users.Impl;

    /// <summary>
    /// Class ProjectServiceTest.
    /// </summary>
    [TestFixture]
    public class UserServiceTest : BaseTest
    {
        private IUsersService userService;
        private IUsersDao usersDao;
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

            DbConnection connection = new SqliteConnection("Data Source=TempProject;Mode=Memory;Cache=Shared");
            connection.Open();
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalOrderValidationDBTest")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.Database.EnsureDeleted();
            this.context.Database.EnsureCreated();

            this.context.Users.AddRange(this.GetAllUserModel());
            this.context.SaveChanges();

            this.usersDao = new UsersDao(this.context);
            this.userService = new UsersService(this.mapper, this.usersDao);
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateGetAllAsync()
        {
            var response = await this.userService.GetAllAsync();

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any());
            Assert.That(response.Count().Equals(8));
        }

        /// <summary>
        /// Method Validate GetByIdAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateGetByIdAsync()
        {
            int id = 1;
            var response = await this.userService.GetByIdAsync(id);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Id.Equals(id));
        }

        /// <summary>
        /// Method Validate InsertAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateInsertAsync()
        {
            var user = "userToken";
            var request = new CreateUserDto()
            {
                Name = "User 10",
                UserName = "user10",
                Email = "user10@yopmail.com",
            };

            var response = await this.userService.InsertAsync(user, request);

            Assert.That(response.Id, Is.Not.Null);
            Assert.That(response.Name.Equals(request.Name));
            Assert.That(response.UserName.Equals(request.UserName));
            Assert.That(response.Email.Equals(request.Email));
        }

        /// <summary>
        /// Method Validate UpdateAsync.
        /// </summary>
        /// <param name="id">Project Id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(1)]
        public async Task ValidateUpdateAsync(int id)
        {
            var user = "userToken";
            var request = new UpdateUserDto()
            {
                Name = "Usuario Uno",
                UserName = "user1",
            };

            var before = await this.userService.GetByIdAsync(id);
            var response = await this.userService.UpdateAsync(id, user, request);

            Assert.That(response.Id.Equals(before.Id));
            Assert.That(response.Name.Equals(request.Name));
            Assert.That(response.UserName.Equals(request.UserName));
            Assert.That(!response.Email.Equals(before.Email));
        }

        /// <summary>
        /// Method Validate DeleteAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateDeleteAsync()
        {
            var id = 9;
            var before = await this.userService.GetByIdAsync(id);
            await this.userService.DeleteAsync(id);

            Assert.That(before, Is.Not.Null);
            Assert.ThrowsAsync<NotFoundException>(async () => await this.userService.GetByIdAsync(id));
        }
    }
}
