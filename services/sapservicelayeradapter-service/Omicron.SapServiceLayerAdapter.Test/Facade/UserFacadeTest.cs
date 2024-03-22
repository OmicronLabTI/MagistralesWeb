// <summary>
// <copyright file="UserFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapServiceLayerAdapter.Test.Facade
{
    using Omicron.SapServiceLayerAdapter.Common.DTOs.Requests.Users;
    using Omicron.SapServiceLayerAdapter.Common.DTOs.Responses.Users;
    using Omicron.SapServiceLayerAdapter.Facade.Users;
    using Omicron.SapServiceLayerAdapter.Facade.Users.Impl;
    using Omicron.SapServiceLayerAdapter.Services.Users;

    /// <summary>
    /// Class ProjectFacadeTest.
    /// </summary>
    [TestFixture]
    public class UserFacadeTest : BaseTest
    {
        private IUsersFacade projectFacade;
        private IUsersService projectService;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ProjectFacadeDB")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var mockProject = new Mock<IUsersService>();
            var userResponse = new UserDto
            {
                Name = "User 1",
                UserName = "user1",
                Email = "user1@yopmail.com",
            };
            IEnumerable<UserDto> listUserResponse =
                new List<UserDto> { userResponse };

            mockProject
                .Setup(m => m.GetAllAsync())
                .Returns(Task.FromResult(listUserResponse));

            mockProject
                .Setup(m => m.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(userResponse));

            mockProject
                .Setup(m => m.InsertAsync(It.IsAny<string>(), It.IsAny<CreateUserDto>()))
                .Returns(Task.FromResult(userResponse));

            mockProject
                .Setup(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<UpdateUserDto>()))
                .Returns(Task.FromResult(userResponse));

            mockProject
                .Setup(m => m.DeleteAsync(It.IsAny<int>()));

            this.projectService = mockProject.Object;
            this.projectFacade = new UsersFacade(this.projectService);
        }

        /// <summary>
        /// ValidateConstructorInvalids.
        /// </summary>
        [Test]
        public void ValidateConstructorInvalids()
        {
            Assert.Throws<ArgumentNullException>(() => new UsersFacade(null));
        }

        /// <summary>
        /// Test for validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task ValidateGetAllAsync()
        {
            var response = await this.projectFacade.GetAllAsync();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Any());
        }

        /// <summary>
        /// Test for validate GetByIdAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task ValidateGetByIdAsync()
        {
            int id = 1;
            var response = await this.projectFacade.GetByIdAsync(id);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test for validate InsertAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task ValidateInsertAsync()
        {
            var request = new CreateUserDto() { Name = "User 1", UserName = "user1", Email = "user1@yopmail.com", };
            var response = await this.projectFacade.InsertAsync("userToken", request);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test for validate UpdateAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task ValidateUpdateAsync()
        {
            var request = new UpdateUserDto() { Name = "User 1", UserName = "user1" };
            var response = await this.projectFacade.UpdateAsync(1, "userToken", request);
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test for validate UpdateAsync BusinessException.
        /// </summary>
        [Test]
        public void ValidateUpdateAsyncBusinessException()
        {
            var messageError = "Ha ocurrido un error.";
            var mockProject = new Mock<IUsersService>();
            mockProject
                .Setup(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<UpdateUserDto>()))
                .ThrowsAsync(new BusinessException(messageError));

            var facade = new UsersFacade(mockProject.Object);
            var request = new UpdateUserDto() { Name = "User 1", UserName = "user1" };
            Assert.ThrowsAsync<BusinessException>(
                async () => await facade.UpdateAsync(1, "userToken", request),
                messageError);
        }

        /// <summary>
        /// Test for validate DeleteAsync.
        /// </summary>
        [Test]
        public void ValidateDelete()
        {
            var id = 1;
            Assert.DoesNotThrowAsync(async () => await this.projectFacade.DeleteAsync(id));
        }
    }
}
