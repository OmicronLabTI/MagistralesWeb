// <summary>
// <copyright file="SapAdapterServiceTest.cs" company="Axity">
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
    public class SapAdapterServiceTest : BaseHttpClientTest<SapAdapterService>
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
        public void GetProductionOrdersByCriterial()
        {
            // Arrange
            var client = this.CreateClient(this.GetProductionOrderModels());

            // Act
            var result = client.GetProductionOrdersByCriterial(new List<int>() { 1, 2 }, new List<int>() { 1, 2 }).Result;

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        private ResultModel GetProductionOrderModels()
        {
            var result = AutoFixtureProvider.Create<ResultModel>();
            result.Success = true;
            result.Response = AutoFixtureProvider.CreateList<ProductionOrderModel>(3);
            return result;
        }
    }
}
