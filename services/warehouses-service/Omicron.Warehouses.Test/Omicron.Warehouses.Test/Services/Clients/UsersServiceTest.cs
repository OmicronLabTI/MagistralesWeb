// <summary>
// <copyright file="UsersServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test.Services.SapAdapter
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Clients;

    /// <summary>
    /// Test class for catalogs service.
    /// </summary>
    [TestFixture]
    public class UsersServiceTest : BaseHttpClientTest<UsersService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetAsync()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetAsync("endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostAsync()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostAsync(new { }, "endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetUsersById()
        {
            // Arrange
            var client = this.CreateClient(this.GetMockUsers());

            // Act
            var result = client.GetUsersById("user-id").Result;

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        private ResultModel GetMockUsers()
        {
            var result = AutoFixtureProvider.Create<ResultModel>();
            result.Success = true;
            result.Response = new List<UserModel>
            {
                new UserModel { Id = "user-id", FirstName = "FirstName", LastName = "LastName", Role = 1, UserName = "UserName" },
            };

            return result;
        }
    }
}
