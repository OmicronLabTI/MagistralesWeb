// <summary>
// <copyright file="CatalogsServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Test.Services.SapAdapter
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Clients;

    /// <summary>
    /// Test class for catalogs service.
    /// </summary>
    [TestFixture]
    public class CatalogsServiceTest : BaseHttpClientTest<CatalogsService>
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
        public void GetParams()
        {
            // Arrange
            var client = this.CreateClient(this.GetMockParamsResult());

            // Act2
            var result = client.GetParams(new List<string> { "p1", "p2" }).Result;

            // Assert
            Assert.AreEqual(8, result.Count);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetRawMaterialEmailConfig()
        {
            // Arrange
            var client = this.CreateClient(this.GetMockParamsResult());

            // Act2
            var result = client.GetRawMaterialEmailConfig().Result;

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetSmtpConfig()
        {
            // Arrange
            var client = this.CreateClient(this.GetMockParamsResult());

            // Act2
            var result = client.GetSmtpConfig().Result;

            // Assert
            Assert.IsNotNull(result);
        }

        private ResultModel GetMockParamsResult()
        {
            var result = AutoFixtureProvider.Create<ResultModel>();
            result.Success = true;
            result.Response = new List<ParametersModel>
            {
                new ParametersModel { Field = "EmailLogisticaCc2", Value = "mail", },
                new ParametersModel { Field = "EmailLogisticaCc1", Value = "mail", },
                new ParametersModel { Field = "EmailAlmacen", Value = "mail", },
                new ParametersModel { Field = "EmailMiddleware", Value = "mail", },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = "password", },
                new ParametersModel { Field = "SmtpServer", Value = "server", },
                new ParametersModel { Field = "SmtpPort", Value = "1", },
                new ParametersModel { Field = "EmailCCDelivery", Value = "asdf" },
            };

            return result;
        }
    }
}
