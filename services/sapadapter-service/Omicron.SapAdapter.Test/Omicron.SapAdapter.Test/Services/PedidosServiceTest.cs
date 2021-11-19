// <summary>
// <copyright file="PedidosServiceTest.cs" company="Axity">
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
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Services.Pedidos;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class PedidosServiceTest
    {
        /// <summary>
        /// Test for simple post.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SimplePost()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    "salesOrder", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidoService(clientMock);

            // act
            var result = await pedidoService.PostPedidos(new List<int>(), "salesOrder");

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test for simple get.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SimpleGet()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    "qfbOrders/", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidoService(clientMock);

            // act
            var result = await pedidoService.GetPedidosService("qfbOrders/");

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test for simple get.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SimpleGetUserPedidos()
        {
            // arrange
            var responses = this.GetMockResponse(HttpStatusCode.OK);

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidoService(clientMock);

            // act
            var result = await pedidoService.GetUserPedidos("qfbOrders/");

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test for simple get.
        /// </summary>
        [Test]
        public void SimpleGetPedidosServiceFailure()
        {
            // arrange
            var responses = this.GetMockResponse(HttpStatusCode.BadRequest);

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidoService(clientMock);

            // assert
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoService.GetPedidosService("qfbOrders/"));
        }

        /// <summary>
        /// Test for simple get.
        /// </summary>
        [Test]
        public void SimpleGetUserPedidosFailure()
        {
            // arrange
            var responses = this.GetMockResponse(HttpStatusCode.BadRequest);

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidoService(clientMock);

            // assert
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoService.GetUserPedidos("qfbOrders/"));
        }

        private Dictionary<string, MockResponse> GetMockResponse(HttpStatusCode statusCode)
        {
           return new Dictionary<string, MockResponse>()
            {
                {
                    "qfbOrders/", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = statusCode,
                    }
                },
            };
        }
    }
}
