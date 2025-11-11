// <summary>
// <copyright file="BackgroundTaskQueueTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.BackgroundTask
{
    /// <summary>
    /// class for test.
    /// </summary>
    public class BackgroundTaskQueueTest
    {
        /// <summary>
        /// class for test.
        /// </summary>
        [TestFixture]
        public class BackgroundTaskQueueTests
        {
            /// <summary>
            /// Action tests.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Test]
            public async Task QueueAndDequeueShouldReturnSameWorkItem()
            {
                // Arrange
                var queue = new BackgroundTaskQueue();
                Func<CancellationToken, Task> workItem = ct => Task.CompletedTask;

                // Act
                queue.QueueBackgroundWorkItem(workItem);
                var dequeued = await queue.DequeueAsync(CancellationToken.None);

                // Assert
                ClassicAssert.NotNull(dequeued);
                ClassicAssert.That(dequeued, Is.EqualTo(workItem));
            }

            /// <summary>
            /// Action tests.
            /// </summary>
            [Test]
            public void QueueBackgroundWorkItemShouldThrowIfNull()
            {
                // Arrange
                var queue = new BackgroundTaskQueue();

                // Act & Assert
                ClassicAssert.Throws<ArgumentNullException>(() => queue.QueueBackgroundWorkItem(null!));
            }

            /// <summary>
            /// Action tests.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Test]
            public async Task DequeueAsyncShouldWaitUntilItemIsQueued()
            {
                // Arrange
                var queue = new BackgroundTaskQueue();
                Func<CancellationToken, Task> workItem = ct => Task.CompletedTask;

                // Act
                var dequeueTask = Task.Run(async () => await queue.DequeueAsync(CancellationToken.None));

                // Esperamos un poco para simular que está bloqueado esperando
                await Task.Delay(100);

                queue.QueueBackgroundWorkItem(workItem);
                var dequeued = await dequeueTask;

                // Assert
                ClassicAssert.That(dequeued, Is.EqualTo(workItem));
            }

            /// <summary>
            /// Action tests.
            /// </summary>
            [Test]
            public void DequeueAsyncShouldThrowWhenCancelled()
            {
                // Arrange
                var queue = new BackgroundTaskQueue();
                using var cts = new CancellationTokenSource();

                // Act
                var task = queue.DequeueAsync(cts.Token);
                cts.Cancel();

                // Assert
                ClassicAssert.ThrowsAsync<OperationCanceledException>(async () => await task);
            }
        }
    }
}
