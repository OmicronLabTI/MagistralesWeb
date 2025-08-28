// <summary>
// <copyright file="CreateChildOrdersSapCommandTest.cs" company="Axity">
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
    public class CreateChildOrdersSapCommandTest
    {
        /// <summary>
        /// Test constructor initializes properties correctly.
        /// </summary>
        [Test]
        public void CreateChildOrdersSapCommand()
        {
            // Arrange
            var productionOrderId = 12345;
            var pieces = 10;
            var separationId = "5451dcbc-ffc3-4874-a3a4-db2a5c6641cf";
            var userId = "1a663b91-fffa-4298-80c3-aaae35586dc6";
            var dxpOrder = "d675db3d-645a-4553-8fcc-4e4466d87a06s";
            var sapOrder = 176307;
            var totalPieces = 20;
            var lastStep = ServiceConstants.SaveHistoryStep;

            // Act
            var command = new CreateChildOrdersSapCommand(
                productionOrderId,
                pieces,
                separationId,
                userId,
                dxpOrder,
                sapOrder,
                totalPieces,
                lastStep);

            // Assert
            Assert.That(command.ProductionOrderId, Is.EqualTo(productionOrderId));
            Assert.That(command.Pieces, Is.EqualTo(pieces));
            Assert.That(command.SeparationId, Is.EqualTo(separationId));
        }

        /// <summary>
        /// Test property setters work correctly.
        /// </summary>
        [Test]
        public void CreateChildOrdersSapCommandPropertiesCanBeSetAndRetrieved()
        {
            // Arrange
            var command = new CreateChildOrdersSapCommand(1, 1, "test", "axity1", "xxx-xxx-xxx", 123, 10, "CancelSAP");

            // Act
            command.ProductionOrderId = 54321;
            command.Pieces = 5;
            command.SeparationId = "new-separation-id";
            command.LastStep = "SaveHistory";

            // Assert
            Assert.That(command.ProductionOrderId, Is.EqualTo(54321));
            Assert.That(command.Pieces, Is.EqualTo(5));
            Assert.That(command.SeparationId, Is.EqualTo("new-separation-id"));
            Assert.That(command.LastStep, Is.EqualTo("SaveHistory"));
        }
    }
}
