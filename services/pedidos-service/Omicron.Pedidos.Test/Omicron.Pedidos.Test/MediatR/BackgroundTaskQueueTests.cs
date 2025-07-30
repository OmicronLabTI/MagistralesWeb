// <summary>
// <copyright file="BackgroundTaskQueueTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    /// <summary>
    /// Tests for BackgroundTaskQueue.
    /// </summary>
    [TestFixture]
    public class BackgroundTaskQueueTests
    {
        private Mock<ILogger> mockLogger;
        private BackgroundTaskQueue queue;

        /// <summary>
        /// Setup for each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockLogger = new Mock<ILogger>();
            this.queue = new BackgroundTaskQueue(this.mockLogger.Object);
        }

        /// <summary>
        /// Test QueueBackgroundWorkItem adds item successfully.
        /// </summary>
        [Test]
        public void QueueBackgroundWorkItemAddsItemSuccessfully()
        {
            // Arrange
            Func<IServiceProvider, CancellationToken, Task> workItem = (sp, ct) => Task.CompletedTask;

            // Act & Assert
            Assert.DoesNotThrow(() => this.queue.QueueBackgroundWorkItem(workItem));
        }

        /// <summary>
        /// Test QueueBackgroundWorkItem throws exception when workItem is null.
        /// </summary>
        [Test]
        public void QueueBackgroundWorkItemThrowsExceptionWhenWorkItemIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => this.queue.QueueBackgroundWorkItem(null));
        }

        /// <summary>
        /// Test DequeueAsync returns queued item.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DequeueAsyncReturnsQueuedItem()
        {
            // Arrange
            Func<IServiceProvider, CancellationToken, Task> workItem = (sp, ct) => Task.CompletedTask;
            this.queue.QueueBackgroundWorkItem(workItem);

            // Act
            var result = await this.queue.DequeueAsync(CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(workItem));
        }

        /// <summary>
        /// Test DequeueAsync waits for item when queue is empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public Task DequeueAsyncWaitsForItemWhenQueueIsEmpty()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await this.queue.DequeueAsync(cancellationTokenSource.Token));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Test multiple items can be queued and dequeued in order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task QueueAndDequeueMultipleItemsMaintainsOrder()
        {
            // Arrange
            Func<IServiceProvider, CancellationToken, Task> workItem1 = (sp, ct) => Task.FromResult(1);
            Func<IServiceProvider, CancellationToken, Task> workItem2 = (sp, ct) => Task.FromResult(2);

            // Act
            this.queue.QueueBackgroundWorkItem(workItem1);
            this.queue.QueueBackgroundWorkItem(workItem2);

            var result1 = await this.queue.DequeueAsync(CancellationToken.None);
            var result2 = await this.queue.DequeueAsync(CancellationToken.None);

            // Assert
            Assert.That(result1, Is.EqualTo(workItem1));
            Assert.That(result2, Is.EqualTo(workItem2));
        }

        /// <summary>
        /// Test constructor throws exception when logger is null.
        /// </summary>
        [Test]
        public void ConstructorThrowsExceptionWhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BackgroundTaskQueue(null));
        }
    }
}
