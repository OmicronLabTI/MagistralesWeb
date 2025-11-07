// <summary>
// <copyright file="RedisServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Test.Services
{
    /// <summary>
    /// Class FavoritiesServiceTest.
    /// </summary>
    [TestFixture]
    public class RedisServiceTest
    {
        /// <summary>
        /// Method to verify Get the labels and containers catalog.
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
            var result = await localService.GetRedisKey("RedisKey");

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Method to verify Get the labels and containers catalog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task WriteRedisKey()
        {
            var mockLogger = new Mock<ILogger>();
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            // act
            var result = await localService.WriteToRedis("RedisTestkey", "RedisTestValue", new TimeSpan(0, 0, 5));

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Method to verify Get the labels and containers catalog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DeleteKeysFromRedis()
        {
            var mockLogger = new Mock<ILogger>();
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var localService = new RedisService(redisMock.Object);

            var key = "TestKey";
            var result = await localService.DeleteKey(key);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result);
        }

        /// <summary>
        /// ReadListAsync_ShouldReturnStoredItems.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task ReadListAsync_ShouldReturnStoredItems()
        {
            // Arrange
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var redisService = new RedisService(redisMock.Object);

            var key = "testKeyredis";
            var storedItems = new List<string> { "Item1", "Item2", "Item3" };
            var redisValues = storedItems.Select(item => (RedisValue)JsonConvert.SerializeObject(item)).ToArray();

            redisDataBase
                .Setup(db => db.ListRangeAsync(key, 0, 2, It.IsAny<CommandFlags>()))
                .ReturnsAsync(redisValues);

            // Act
            var result = await redisService.ReadListAsync<string>(key, 0, 3);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result, Is.EqualTo(storedItems));

            redisDataBase.Verify(db => db.ListRangeAsync(key, 0, 2, It.IsAny<CommandFlags>()), Times.Once);
        }

        /// <summary>
        /// ReadListAsync_ShouldReturnStoredItems.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task ReadListAsync_ShouldReturnEmptyList_WhenNoDataInRedis()
        {
            // Arrange
            var redisMock = new Mock<IConnectionMultiplexer>();
            var redisDataBase = new Mock<IDatabase>();
            redisMock.Setup(m => m.IsConnected).Returns(true);
            redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDataBase.Object);

            var redisService = new RedisService(redisMock.Object);
            var key = "testKey";

            redisDataBase
                .Setup(db => db.ListRangeAsync(key, 0, 2, It.IsAny<CommandFlags>()))
                .ReturnsAsync(Array.Empty<RedisValue>());

            // Act
            var result = await redisService.ReadListAsync<string>(key, 0, 3);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            redisDataBase.Verify(db => db.ListRangeAsync(key, 0, 2, It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}
