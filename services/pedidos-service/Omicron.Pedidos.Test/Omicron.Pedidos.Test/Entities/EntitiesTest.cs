// <summary>
// <copyright file="EntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test.Entities
{
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
            new DeliveryDetailModel(),
            new FinalizaGeneratePdfModel(),
            new ParametersModel(),
            new UpdateDesignerLabelDetailModel(),
            new UpdateDesignerLabelDetailDto(),
            new UserOrderDto(),
            new ProductionRemisionQrModel(),
            new ProductionFacturaQrModel(),
            new AlmacenGraphicsCount(),
            new AsesorModel(),
            new DetallePedidoModel(),
            new SalesLogs(),
            new ProductionOrderQr(),
            new RelationDxpDocEntryModel(),
            new RelationOrderAndTypeModel(),
            new QfbTecnicInfoDto(),
            new CreateOrderPdfDto(),
            new UserOrdersForInvoicesModel(),
            new UserOrderByInvoiceAndLineNum(),
            new ProductionOrderProcessingStatusModel(),
            new FinalizeProductionOrderModel(),
            new FinalizeProductionOrderPayload(),
            new ProductionOrderFailedResultModel(),
            new ValidateProductionOrderModel(),
            new ValidationsToFinalizeProductionOrdersResultModel(),
            new ProductionOrderProcessingStatusDto(),
            new FinalizeProductionOrdersResult(),
            new RetryFailedProductionOrderFinalizationDto(),
            new CancelProductionOrderDto(),
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
            Assert.That(IsValid(instance));
        }

        private static bool IsValid<T>(T instance)
        {
            Assert.That(instance, Is.Not.Null);
            foreach (var prop in instance.GetType().GetProperties())
            {
                Assert.That(GetPropValue(instance, prop.Name), Is.Not.Null);
            }

            return true;
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
