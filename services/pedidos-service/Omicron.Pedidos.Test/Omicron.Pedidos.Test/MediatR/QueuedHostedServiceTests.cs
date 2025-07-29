// <summary>
// <copyright file="QueuedHostedServiceTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    /// <summary>
    /// Tests for QueuedHostedService.
    /// </summary>
    [TestFixture]
    public class QueuedHostedServiceTests
    {
        private Mock<IBackgroundTaskQueue> mockTaskQueue;
        private Mock<ILogger> mockLogger;
        private Mock<IServiceProvider> mockServiceProvider;
        private QueuedHostedService service;

        /// <summary>
        /// Setup for each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            this.mockLogger = new Mock<ILogger>();
            this.mockServiceProvider = new Mock<IServiceProvider>();

            this.service = new QueuedHostedService(
                this.mockTaskQueue.Object,
                this.mockLogger.Object,
                this.mockServiceProvider.Object);
        }

        /// <summary>
        /// Test StopAsync stops the service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public Task StopAsyncStopsService()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await this.service.StopAsync(cancellationToken));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Test constructor throws exception when logger is null.
        /// </summary>
        [Test]
        public void ConstructorThrowsExceptionWhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new QueuedHostedService(this.mockTaskQueue.Object, null, this.mockServiceProvider.Object));
        }

        /// <summary>
        /// Test constructor with valid parameters.
        /// </summary>
        [Test]
        public void ConstructorWithValidParametersCreatesInstance()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
                new QueuedHostedService(this.mockTaskQueue.Object, this.mockLogger.Object, this.mockServiceProvider.Object));
        }
    }
}
