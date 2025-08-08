// <summary>
// <copyright file="CommandTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    /// <summary>
    /// Tests for CommandTest.
    /// </summary>
    [TestFixture]
    public class CommandTest
    {
        /// <summary>
        /// Test constructor initializes properties correctly.
        /// </summary>
        [Test]
        public void SeparateProductionOrderCommand()
        {
            // Arrange
            var productionOrderId = 12345;
            var pieces = 100;
            var separationId = "test-separation-id";
            var userId = "axity1";
            var dxpOrder = "xxx-xxx";
            var sapOrder = 1212;
            var totalPieces = 1;

            // Act
            var command = new SeparateProductionOrderCommand(
                productionOrderId,
                pieces,
                separationId,
                userId,
                dxpOrder,
                sapOrder,
                totalPieces);

            // Assert
            Assert.That(command.ProductionOrderId, Is.EqualTo(productionOrderId));
            Assert.That(command.Pieces, Is.EqualTo(pieces));
            Assert.That(command.SeparationId, Is.EqualTo(separationId));
            Assert.That(command.RetryCount, Is.EqualTo(0));
            Assert.That(command.MaxRetries, Is.EqualTo(3));
        }

        /// <summary>
        /// Test property setters work correctly.
        /// </summary>
        [Test]
        public void SeparateProductionOrderCommandPropertiesCanBeSetAndRetrieved()
        {
            // Arrange
            var command = new SeparateProductionOrderCommand(1, 1, "test", "axity1", "xxx-xxx-xxx", 123, 10);

            // Act
            command.ProductionOrderId = 54321;
            command.Pieces = 200;
            command.SeparationId = "new-separation-id";
            command.RetryCount = 2;
            command.MaxRetries = 5;

            // Assert
            Assert.That(command.ProductionOrderId, Is.EqualTo(54321));
            Assert.That(command.Pieces, Is.EqualTo(200));
            Assert.That(command.SeparationId, Is.EqualTo("new-separation-id"));
            Assert.That(command.RetryCount, Is.EqualTo(2));
            Assert.That(command.MaxRetries, Is.EqualTo(5));
        }

        /// <summary>
        /// Test default values for optional properties.
        /// </summary>
        [Test]
        public void SeparateProductionOrderCommandConstructorSetsDefaultValues()
        {
            // Act
            var command = new SeparateProductionOrderCommand(1, 10, "test-id", "axity1", "xxx-xxx-xxx", 123, 10);

            // Assert
            Assert.That(command.RetryCount, Is.EqualTo(0));
            Assert.That(command.MaxRetries, Is.EqualTo(3));
        }

        /// <summary>
        /// Test constructor initializes properties correctly.
        /// </summary>
        [Test]
        public void StartProductionOrderSeparationCommand()
        {
            // Arrange
            var productionOrderId = 12345;
            var pieces = 100;
            var separationId = "test-separation-id";
            var userId = "axity1";
            var dxpOrder = "xx-xxx-xxx";
            var sapOrder = 123;
            var totalPieces = 10;

            // Act
            var command = new StartProductionOrderSeparationCommand(
                productionOrderId,
                pieces,
                separationId,
                userId,
                dxpOrder,
                sapOrder,
                totalPieces);

            // Assert
            Assert.That(command.ProductionOrderId, Is.EqualTo(productionOrderId));
            Assert.That(command.Pieces, Is.EqualTo(pieces));
            Assert.That(command.SeparationId, Is.EqualTo(separationId));
        }

        /// <summary>
        /// Test property setters work correctly.
        /// </summary>
        [Test]
        public void StartProductionOrderSeparationCommandPropertiesCanBeSetAndRetrieved()
        {
            // Arrange
            var command = new StartProductionOrderSeparationCommand(1, 1, "test", "axity1", "xxx-xxx-xxx", 123, 10);

            // Act
            command.ProductionOrderId = 54321;
            command.Pieces = 200;
            command.SeparationId = "new-separation-id";

            // Assert
            Assert.That(command.ProductionOrderId, Is.EqualTo(54321));
            Assert.That(command.Pieces, Is.EqualTo(200));
            Assert.That(command.SeparationId, Is.EqualTo("new-separation-id"));
        }
    }
}
