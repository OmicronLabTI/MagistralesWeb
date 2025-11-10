// <summary>
// <copyright file="KafkaConnectorTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Test.Services
{
    /// <summary>
    /// KafkaConnectorTest.
    /// </summary>
    [TestFixture]
    public class KafkaConnectorTest
    {
        private Mock<IConfiguration> configurationMock;
        private Mock<IConfigurationSection> sectionMock;
        private Mock<Serilog.ILogger> loggerMock;
        private Mock<IProducer<long, string>> producerMock;
        private KafkaConnector service;

        /// <summary>
        /// Setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.configurationMock = new Mock<IConfiguration>();
            this.sectionMock = new Mock<IConfigurationSection>();
            this.loggerMock = new Mock<Serilog.ILogger>();
            this.producerMock = new Mock<IProducer<long, string>>();

            this.sectionMock.Setup(x => x["EH_NAME"]).Returns("topic-test");
            this.sectionMock.Setup(x => x["EH_FQDN"]).Returns("localhost:9092");
            this.sectionMock.Setup(x => x["EH_CONNECTION_STRING"]).Returns("Endpoint=sb://test");

            this.configurationMock.Setup(x => x.GetSection("RetryFinalizeProductionOrder")).Returns(this.sectionMock.Object);

            this.service = new KafkaConnector(this.configurationMock.Object, this.loggerMock.Object);
        }

        /// <summary>
        /// PushMessageShouldReturnTrueWhenProduceSuccess.
        /// </summary>
        /// <param name="environment">Environment.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase("Uat")]
        [TestCase("Development")]
        public async Task PushMessageShouldReturnTrueWhenProduceSuccess(string environment)
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            var producerMock = new Mock<IProducer<long, string>>();

            producerMock
                .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<long, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeliveryResult<long, string>());

            // setup config & logger mocks (omito por brevedad)
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.Setup(s => s["EH_NAME"]).Returns("topic-test");
            sectionMock.Setup(s => s["EH_FQDN"]).Returns("localhost:9092");
            sectionMock.Setup(s => s["EH_CONNECTION_STRING"]).Returns("Endpoint=sb://test");
            configMock.Setup(c => c.GetSection("RetryFinalizeProductionOrder")).Returns(sectionMock.Object);

            var loggerMock = new Mock<Serilog.ILogger>();

            var service = new KafkaConnector(configMock.Object, loggerMock.Object);

            // Reemplazamos la factory para devolver el mock
            service.BuildProducerFunc = cfg => producerMock.Object;

            // Act
            var result = await service.PushMessage(new { Test = "ABC" }, "LOGBASE");

            // Assert
            Assert.That(result, Is.True);
            producerMock.Verify(p => p.ProduceAsync("topic-test", It.IsAny<Message<long, string>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// PushMessageShouldReturnFalseWhenProduceThrows.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PushMessageShouldReturnFalseWhenProduceThrows()
        {
            var configMock = new Mock<IConfiguration>();
            var producerMock = new Mock<IProducer<long, string>>();
            producerMock
                .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<long, string>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error"));

            var service = new KafkaConnector(configMock.Object, this.loggerMock.Object);
            service.BuildProducerFunc = cfg => producerMock.Object;

            var result = await service.PushMessage(new { Test = "ABC" }, "LOGBASE");

            Assert.That(result, Is.False);
            this.loggerMock.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
