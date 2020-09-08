// <summary>
// <copyright file="EntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test.Entities
{
    using AutoFixture;
    using NUnit.Framework;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Class for tests entities.
    /// </summary>
    [TestFixture]
    public class EntitiesTest
    {
        /// <summary>
        /// TypeCases.
        /// </summary>
        private static readonly object[] TypeCases =
        {
            new FabOrderDetail(),
            new FabricacionOrderModel(),
            new FinishOrderModel(),
            new GetOrderFabModel(),
            new ManualAssignModel(),
            new OrderIdModel(),
            new OrderModel(),
            new OrderWithDetailModel(),
            new ProcessByOrderModel(),
            new ProcessOrderModel(),
            new ProductivityModel(),
            new QfbOrderDetail(),
            new QfbOrderModel(),
            new ResultModel(),
            new SuccessFailResults<OrderIdModel>(),
            new UpdateFabOrderModel(),
            new UpdateOrderCommentsModel(),
            new UpdateOrderSignatureModel(),
            new UpdateStatusOrderModel(),
            new UserModel(),
            new WorkLoadModel(),
            new UserOrderSignatureModel(),
            new OrderLogModel(),
            new UpdateFormulaModel(),
            new CompleteFormulaWithDetalle(),
            new CompleteOrderModel(),
            new CreateIsolatedFabOrderModel(),
            new CompleteDetalleFormulaModel(),
            new CompleteDetailOrderModel(),
            new CloseProductionOrderModel(),
            new BatchesConfigurationModel(),
            new BatchesComponentModel(),
            new AssignBatchModel(),
            new AssignedBatches(),
            new AutomaticAssignUserModel(),
            new AutomaticAssingModel(),
            new CustomComponentListModel(),
            new ComponentCustomComponentListModel(),
            new RawMaterialRequestModel(),
            new RawMaterialRequestDetailModel(),
        };

        /// <summary>
        /// The fixture.
        /// </summary>
        private readonly Fixture fixture = new Fixture();

        /// <summary>
        /// Validate instance for type.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="instance">Type.</param>
        /// [AutoData]
        [Test]
        [TestCaseSource("TypeCases")]
        public void TestInstance<T>(T instance)
            where T : class
        {
            // Arrange
            instance = this.fixture.Create<T>();

            // Assert
            Assert.IsTrue(IsValid(instance));
        }

        private static bool IsValid<T>(T instance)
        {
            Assert.IsNotNull(instance);
            foreach (var prop in instance.GetType().GetProperties())
            {
                Assert.IsNotNull(GetPropValue(instance, prop.Name));
            }

            return true;
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
