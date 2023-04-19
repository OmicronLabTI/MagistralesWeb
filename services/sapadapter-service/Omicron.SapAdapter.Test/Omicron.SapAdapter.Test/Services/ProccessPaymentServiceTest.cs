// <summary>
// <copyright file="ProccessPaymentServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Serilog;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class ProccessPaymentServiceTest
    {
        /// <summary>
        /// Test for simple post.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task PostProccessPayments()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    "endpoint", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.Information(It.IsAny<string>()));

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new ProccessPayments(clientMock, mockLogger.Object);

            // act
            var result = await pedidoService.PostProccessPayments(new List<int>(), "endpoint");

            // assert
            Assert.IsNotNull(result);
        }
    }
}
