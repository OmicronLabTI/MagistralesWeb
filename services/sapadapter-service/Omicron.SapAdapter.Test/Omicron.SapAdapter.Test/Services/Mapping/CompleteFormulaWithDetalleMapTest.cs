// <summary>
// <copyright file="CompleteFormulaWithDetalleMapTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Test.Services.Mapping
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Mapping;

    /// <summary>
    /// Test for <see cref="CompleteFormulaWithDetalleMap" />.
    /// </summary>
    [TestFixture]
    public class CompleteFormulaWithDetalleMapTest
    {
        /// <summary>
        /// Map OrdenFabricacionModel to CompleteFormulaWithDetalle.
        /// </summary>
        [Test]
        public void Map_OrdenFabricacionModel_MapCorrectly()
        {
            // Arrange
            var baseObject = new CompleteFormulaWithDetalle();
            var objectToMap = AutoFixtureProvider.Create<OrdenFabricacionModel>();

            // Act
            baseObject.Map(objectToMap);

            // Assert
            Assert.AreEqual(objectToMap.OrdenId, baseObject.ProductionOrderId);
            Assert.AreEqual(objectToMap.ProductoId, baseObject.Code);
            Assert.AreEqual(objectToMap.Wharehouse, baseObject.Warehouse);
            Assert.AreEqual(objectToMap.Unit, baseObject.Unit);
            Assert.AreEqual(objectToMap.Status, baseObject.Status);
        }

        /// <summary>
        /// Map List CompleteDetalleFormulaModel to CompleteFormulaWithDetalle.
        /// </summary>
        [Test]
        public void Map_ListCompleteDetalleFormulaModel_MapCorrectly()
        {
            // Arrange
            var baseObject = new CompleteFormulaWithDetalle();
            var objectToMap = AutoFixtureProvider.CreateList<CompleteDetalleFormulaModel>(2);

            // Act
            baseObject.Map(objectToMap);

            // Assert
            Assert.AreEqual(objectToMap.Count, baseObject.Details.Count);
        }

        /// <summary>
        /// Map UserOrderModel to CompleteFormulaWithDetalle.
        /// </summary>
        [Test]
        public void Map_UserOrderModel_MapCorrectly()
        {
            // Arrange
            var baseObject = new CompleteFormulaWithDetalle();
            var objectToMap = AutoFixtureProvider.Create<UserOrderModel>();

            // Act
            baseObject.Map(objectToMap);

            // Assert
            Assert.AreEqual(objectToMap.Status, baseObject.Status);
            Assert.AreEqual(objectToMap.CloseDate, baseObject.RealEndDate);
            Assert.AreEqual(objectToMap.Comments, baseObject.Comments);
        }
    }
}
