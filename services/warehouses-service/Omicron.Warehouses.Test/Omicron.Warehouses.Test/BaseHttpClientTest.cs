// <summary>
// <copyright file="BaseHttpClientTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Moq;
    using Moq.Protected;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Base class for http clients.
    /// </summary>
    /// <typeparam name="T">Client type.</typeparam>
    public abstract class BaseHttpClientTest<T>
        where T : class
    {
        private Mock<HttpMessageHandler> handlerMock;

        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <param name="mockResult">Mock result.</param>
        /// <returns>Client.</returns>
        public T CreateClient(ResultModel mockResult = null)
        {
            var fixture = new Fixture();

            var result = fixture.Create<ResultModel>();
            result.Success = true;

            this.handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            this.handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(mockResult ?? result)),
               })
               .Verifiable();

            var httpClient = new HttpClient(this.handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            return (T)Activator.CreateInstance(typeof(T), new object[] { httpClient });
        }

        /// <summary>
        /// Get a http mock.
        /// </summary>
        /// <returns>Http mock.</returns>
        public Mock<HttpMessageHandler> GetHttpMock()
        {
            return this.handlerMock;
        }
    }
}
