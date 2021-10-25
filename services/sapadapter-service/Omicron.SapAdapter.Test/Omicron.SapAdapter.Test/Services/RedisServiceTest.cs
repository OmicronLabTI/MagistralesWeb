// <summary>
// <copyright file="RedisServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.Services.Redis;
    using Serilog;
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
        /// <param name="valueToReturn">the value to return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(null)]
        [TestCase("1234")]
        public async Task GetRedisKey(string valueToReturn)
        {
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisDataBase
                .Setup(s => s.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(new RedisValue(valueToReturn)));

            redisMock
                .Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(redisDataBase.Object);

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
        public async Task WriteRedisKey()
        {
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            // act
            var result = await localService.WriteToRedis("C001", "C001", new TimeSpan(0, 0, 5));

            // Assert
            Assert.IsTrue(result);
        }
    }
}
