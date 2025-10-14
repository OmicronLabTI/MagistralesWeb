// <summary>
// <copyright file="EntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test
{
    /// <summary>
    /// Class FavoritiesServiceTest.
    /// </summary>
    [TestFixture]
    public class EntitiesTest
    {
        /// <summary>
        /// TypeCases.
        /// </summary>
        private static readonly object[] TypeCases =
        {
            new OrderDto(),
            new OrderLineDto(),
            new ResultDto(),
            new ResultModel(),
            new ServiceLayerAuthResponseDto(),
            new ServiceLayerErrorDetailDto(),
            new ServiceLayerErrorMessageDto(),
            new ServiceLayerErrorResponseDto(),
            new ServiceLayerResponseDto(),
            new InvoiceDto(),
            new InvoiceLineDto(),
            new InvoiceLineDto(),
            new TrackingInformationDto(),
            new ShippingTypesResponseDto(),
            new BatchNumbersDto(),
            new CreateDeliveryDto(),
            new InventoryGenExitDto(),
            new InventoryGenExitLineDto(),
            new CloseSampleOrderDto(),
            new ProductDeliveryDto(),
            new CancelDeliveryDto(),
            new PrescriptionServerRequestDto(),
            new PrescriptionServerResponseDto(),
            new AttachmentDto(),
            new CreateAttachmentDto(),
            new CreateAttachmentResponseDto(),
            new AlmacenBatchDto(),
            new CreateDeliveryNoteDto(),
            new DeliveryNoteBatchNumbersDto(),
            new DeliveryNoteDto(),
            new DeliveryNoteLineDto(),
            new CreateOrderDto(),
            new CreateOrderLineDto(),
            new CreateSaleOrderDto(),
            new OrderLineDto(),
            new ShoppingCartItemDto(),
            new StockTransferDto(),
            new StockTransferLineDto(),
            new AdviserProfileInfoDto(),
            new EmployeeInfoDto(),
            new DoctorAddressDto(),
            new DoctorContactEmployee(),
            new DoctorDeliveryAddressDto(),
            new DoctorDto(),
            new DoctorEmployeeDto(),
            new DoctorInvoiceAddressDto(),
            new DoctorProfileInfoDto(),
            new BusinessPartnerProfileInfoDto(),
            new DoctorDefaultAddressDto(),
            new BusinessPartnerDefaultAddressDto(),
            new DoctorElectronicProtocolDto(),
            new DoctorPaymentMethodDto(),
            new BaseOrderDto(),
            new TransferRequestDetailDto(),
            new TransferRequestHeaderDto(),
            new InventoryTransferRequestResult(),
            new InventoryTransferRequestsDto(),
            new InventoryTransferRequestsResponseDto(),
            new StockTransferLinesDto(),
            new BatchNumberDetailDto(),
            new BatchNumberResponseDto(),
            new BatchInventoryGenEntryDto(),
            new InventoryGenEntryDto(),
            new InventoryGenEntryLineDto(),
            new ItemWarehouseInfoDto(),
            new BatchesConfigurationDto(),
            new CloseProductionOrderDto(),
            new ProductionOrderDto(),
            new ProductionOrderItemBatchDto(),
            new ProductionOrderLineDto(),
            new AssignBatchDto(),
            new CompleteDetalleFormulaDto(),
            new UpdateFormulaDto(),
            new BaseProductionOrder(),
            new CancelOrderDto(),
            new CompleteDetailDto(),
            new CreateOrderRequestDto(),
            new CreateProductionOrderDto(),
            new OrderWithDetailDto(),
            new ProductionOrderDto(),
            new UpdateFabOrderDto(),
            new UpdateFormulaDto(),
            new CreateIsolatedFabOrderDto(),
            new CreateIsolateProductionOrderDto(),
            new BaseBatchProductionOrderDto(),
            new BaseCreateProductionOrderLineDto(),
            new UpdateProductionOrderDto(),
            new ValidationsToFinalizeProductionOrdersResultDto(),
            new CancelProductionOrderDto(),
            new CreateChildOrderResultDto(),
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
            this.fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            this.fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Arrange
            instance = this.fixture.Create<T>();

            // Assert
            Assert.That(IsValid(instance), Is.True);
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
