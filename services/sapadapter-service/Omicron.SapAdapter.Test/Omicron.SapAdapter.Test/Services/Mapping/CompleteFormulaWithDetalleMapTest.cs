// <summary>
// <copyright file="CompleteFormulaWithDetalleMapTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Test.Services.Mapping
{
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
            ClassicAssert.AreEqual(objectToMap.OrdenId, baseObject.ProductionOrderId);
            ClassicAssert.AreEqual(objectToMap.ProductoId, baseObject.Code);
            ClassicAssert.AreEqual(objectToMap.Wharehouse, baseObject.Warehouse);
            ClassicAssert.AreEqual(objectToMap.Unit, baseObject.Unit);
            ClassicAssert.AreEqual(objectToMap.Status, baseObject.Status);
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
            ClassicAssert.AreEqual(objectToMap.Count, baseObject.Details.Count);
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
            ClassicAssert.AreEqual(objectToMap.Status, baseObject.Status);
            ClassicAssert.AreEqual(objectToMap.Comments, baseObject.Comments);
        }
    }
}
