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
        };

        /// <summary>
        /// The fixture.
        /// </summary>
        private readonly Fixture fixture = new ();

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
