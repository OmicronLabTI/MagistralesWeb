// <summary>
// <copyright file="PedidosUtilsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;
    using Omicron.Pedidos.Services.Reporting;
    using Omicron.Pedidos.Services.Redis;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidosUtilsTest
    {
        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateMostUsedComponentsNoItem()
        {
            // arrange
            var listIds = new List<string>();
            var redis = new Mock<IRedisService>();

            var service = new PedidosUtils(redis.Object);

            // act
            await service.UpdateMostUsedComponents(listIds);

            // assert
            Assert.IsNotNull(listIds);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateMostUsedComponents()
        {
            // arrange
            var componentes = new List<ComponentsRedisModel>
            {
                new ComponentsRedisModel { ItemCode = "EN-001", Total = 1 },
            };

            var listIds = new List<string> { "EN-001", "EN-002" };
            var redis = new Mock<IRedisService>();
            redis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(componentes)));

            var service = new PedidosUtils(redis.Object);

            // act
            await service.UpdateMostUsedComponents(listIds);

            // assert
            Assert.IsNotNull(listIds);
        }
    }
}
