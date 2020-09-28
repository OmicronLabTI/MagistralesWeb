// <summary>
// <copyright file="UserServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services.UserService
{
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// Test for user service.
    /// </summary>
    public class UserServiceTest : BaseHttpClientTest<UsersService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void SimpleGetUsers()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.SimpleGetUsers("endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void SimpleGetUsersFailure()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.SimpleGetUsers("endpoint"));
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostSimpleUsers()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostSimpleUsers(new { }, "endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}
