// <summary>
// <copyright file="PedidosUtilsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
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
            await service.UpdateMostUsedComponents(listIds, "redisComponents");

            // assert
            ClassicAssert.IsNotNull(listIds);
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
            await service.UpdateMostUsedComponents(listIds, "redisComponents");

            // assert
            ClassicAssert.IsNotNull(listIds);
        }
    }
}
