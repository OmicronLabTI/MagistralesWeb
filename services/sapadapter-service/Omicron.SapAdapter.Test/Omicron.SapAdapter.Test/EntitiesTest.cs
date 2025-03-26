// <summary>
// <copyright file="EntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
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
            new AlmacenSalesHeaderModel(),
            new AlmacenSalesModel(),
            new DeliveryScannedModel(),
            new InvoiceDeliverModel(),
            new InvoicePackageSapLookDto(),
            new InvoiceProductModel(),
            new InvoiceSaleHeaderModel(),
            new LineProductBatchesModel(),
            new LineScannerModel(),
            new MagistralScannerModel(),
            new OrderRecipeModel(),
            new PackageModel(),
            new SalesModel(),
            new SapIdsModel(),
            new UserOrderDto(),
            new ProductListModel(),
            new UserOrderDto(),
            new IncidentInfoModel(),
            new ParametersModel(),
            new OrderValidationResponse(),
            new Repartidores(),
            new InvoiceDetailModel(),
            new DetallePedidoModel(),
            new AlmacenOrdersModel(),
            new DeliverModel(),
            new DeliveryDetailModel(),
            new InvoiceOrderModel(),
            new AttachmentModel(),
            new CompleteDetailOrderModel(),
            new Batches(),
            new CompleteOrderModel(),
            new ValidBatches(),
            new CompleteFormulaWithDetalle(),
            new OrdenFabricacionModel(),
            new OrdersActivesDto(),
            new AlmacenDetailsOrder(),
            new AlmacenSalesByDoctorHeaderModel(),
            new AlmacenSalesByDoctorModel(),
            new ExclusivePartnersModel(),
            new InvoiceSaleModel(),
            new OrderListByDoctorModel(),
            new InvoiceHeaderAdvancedLookUp(),
            new SalesByDoctorModel(),
            new SaleOrderByDeliveryModel(),
            new AlmacenOrdersByDoctorModel(),
            new InvoiceHeaderModel(),
            new InvoiceDeliveryModel(),
            new OrderModel(),
            new DoctorOrdersSearchDeatilDto(),
            new CompleteDeliveryDetailModel(),
            new AlmacenGetRecepcionModel(),
            new CatalogProductModel(),
            new ClientCatalogModel(),
            new DoctorInfoModel(),
            new ReceipcionPedidosDetailModel(),
            new InvoicesModel(),
            new RetrieveInvoiceModel(),
            new CompleteAlmacenOrderModel(),
            new SaleOrderTypeModel(),
            new DeliveyJoinOrderModel(),
            new SalesPersonModel(),
            new AssignedBatches(),
            new PaymentsDto(),
            new GetDoctorAddressModel(),
            new DoctorAddressModel(),
            new BoxModel(),
            new LineProductWithCodeBarsModel(),
            new DoctorPrescriptionInfoModel(),
            new CountDxpOrders(),
            new RelationDxpDocEntry(),
            new RelationOrderAndTypeModel(),
            new CountDxpOrdersDetail(),
            new DoctorShippingAddressRelationModel(),
            new RawMaterialRequestDetailModel(),
            new RawMaterialRequestModel(),
            new CompleteRawMaterialRequestModel(),
            new OrdersFilterDto(),
            new WarehouseModel(),
            new ActiveWarehouseDto(),
            new WarehouseDto(),
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
