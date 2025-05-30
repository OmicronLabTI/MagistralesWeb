// <summary>
// <copyright file="RedisServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Services
{
    /// <summary>
    /// Test to verify service.
    /// </summary>
    [TestFixture]
    public class RedisServiceTest
    {
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

            var localService = new RedisService(mockLogger.Object, redisMock.Object);

            // act
            var result = await localService.WriteToRedis("catalog", "catalog", new TimeSpan(0, 0, 5));

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
