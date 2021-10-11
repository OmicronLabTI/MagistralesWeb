// <summary>
// <copyright file="EntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    using AutoFixture;
    using NUnit.Framework;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

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
