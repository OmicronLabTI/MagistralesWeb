// <summary>
// <copyright file="StartProductionOrderSeparationHandlerTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    /// <summary>
    /// Tests for StartProductionOrderSeparationHandler.
    /// </summary>
    [TestFixture]
    public class StartProductionOrderSeparationHandlerTests
    {
        private Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue;
        private Mock<ILogger> mockLogger;
        private StartProductionOrderSeparationHandler handler;

        /// <summary>
        /// Setup for each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            this.mockLogger = new Mock<ILogger>();
            this.handler = new StartProductionOrderSeparationHandler(
                this.mockBackgroundTaskQueue.Object,
                this.mockLogger.Object);
        }

        /// <summary>
        /// Test Handle queues background task correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleQueuesBackgroundTaskCorrectly()
        {
            // Arrange
            var command = new StartProductionOrderSeparationCommand(12345, 100, "test-separation-id");
            var cancellationToken = CancellationToken.None;

            // Act
            await this.handler.Handle(command, cancellationToken);

            // Assert
            this.mockBackgroundTaskQueue.Verify(
                x => x.QueueBackgroundWorkItem(It.IsAny<Func<IServiceProvider, CancellationToken, Task>>()),
                Times.Once);
        }

        /// <summary>
        /// Test Handle completes successfully.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public Task HandleCompletesSuccessfully()
        {
            // Arrange
            var command = new StartProductionOrderSeparationCommand(12345, 100, "test-separation-id");
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await this.handler.Handle(command, cancellationToken));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Test constructor throws exception when logger is null.
        /// </summary>
        [Test]
        public void ConstructorThrowsExceptionWhenLoggerIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new StartProductionOrderSeparationHandler(this.mockBackgroundTaskQueue.Object, null));
        }
    }
}
