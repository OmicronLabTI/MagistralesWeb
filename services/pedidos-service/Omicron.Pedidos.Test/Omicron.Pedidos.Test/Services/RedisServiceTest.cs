// <summary>
// <copyright file="RedisServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.Services.Redis;
    using StackExchange.Redis;

    /// <summary>
    /// Class FavoritiesServiceTest.
    /// </summary>
    [TestFixture]
    public class RedisServiceTest
    {
        /// <summary>
        /// Method to verify Get All Favoritiess.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRedisKey()
        {
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            // act
            var result = await localService.GetRedisKey("C001");

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to verify Get All Favoritiess.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task WriteRedisKeyWithTtl()
        {
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            // act
            var result = await localService.WriteToRedis("C001", "C001", new TimeSpan(0, 0, 5));

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to verify Get All Favoritiess.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task WriteRedisKey()
        {
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            // act
            var result = await localService.WriteToRedis("C001", "C001");

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to verify Get All Favoritiess.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DeleteRedisKey()
        {
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            // act
            var result = await localService.DeleteKey("C001");

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
