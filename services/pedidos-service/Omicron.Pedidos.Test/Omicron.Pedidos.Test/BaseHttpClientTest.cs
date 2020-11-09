// <summary>
// <copyright file="BaseHttpClientTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Moq;
    using Moq.Protected;
    using Omicron.Pedidos.Entities.Model;
    using Serilog;

    /// <summary>
    /// Base class for http clients.
    /// </summary>
    /// <typeparam name="T">Client type.</typeparam>
    public abstract class BaseHttpClientTest<T>
        where T : class
    {
        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <returns>Client.</returns>
        public T CreateClient()
        {
            var fixture = new Fixture();
            var result = fixture.Create<ResultModel>();
            result.Success = true;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result)),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            return (T)Activator.CreateInstance(typeof(T), new object[] { httpClient, mockLog.Object });
        }

        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <returns>Client.</returns>
        public T CreateClientFailure()
        {
            var fixture = new Fixture();
            var result = fixture.Create<ResultModel>();
            result.Success = true;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest,
                   Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result)),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            return (T)Activator.CreateInstance(typeof(T), new object[] { httpClient, mockLog.Object });
        }
    }
}
