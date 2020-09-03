// <summary>
// <copyright file="PedidosServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Services.Constants;
    using Omicron.Usuarios.Services.Pedidos;

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
                    ServiceConstants.QfbOrders, new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidosService(clientMock);

            // act
            var result = await pedidoService.PostPedidos(new List<int>(), ServiceConstants.QfbOrders);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test for simple post.
        /// </summary>
        [Test]
        public void SimplePostError()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    ServiceConstants.QfbOrders, new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.BadRequest,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new PedidosService(clientMock);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoService.PostPedidos(new List<int>(), ServiceConstants.QfbOrders));
        }
    }
}
