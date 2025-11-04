// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Test
{
    using System.Collections.Generic;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Get GetInvoiceModel.
        /// </summary>
        /// <returns>The GetInvoiceModel.</returns>
        public IEnumerable<InvoiceModel> GetInvoiceModel()
            => new List<InvoiceModel>()
            {
                // RETRY
                new ()
                {
                    Id = "bc261af6-682b-4f29-ac3d-74a1b69129fd", CreateDate = DateTime.UtcNow, Payload = @"{""CardCode"": ""C000123"",
                      ""ProcessId"": ""bc261af6-682b-4f29-ac3d-74a1b69129fd"",
                      ""CfdiDriverVersion"": ""4.0"",
                      ""IdDeliveries"": [ 10, 11, 12 ],
                      ""IdSapOrders"": [ 555, 556, 557 ],
                      ""CreateUserId"": """",
                      ""DxpOrderId"": ""58550c92-f0fc-4528-80ef-f4cfa93a095f"",
                      ""InvoiceType"": ""complete"",
                      ""BillingType"": ""complete""
                    }", IsProcessing = false, Status = "Error al crear", IdInvoiceError = 1,
                },
                new ()
                {
                    Id = "eb3aa587-775f-43ce-ac3d-e09dd0f4bdc2", CreateDate = DateTime.UtcNow.AddMinutes(-1), Payload = @"{""CardCode"": ""C000123"",
                      ""ProcessId"": ""eb3aa587-775f-43ce-ac3d-e09dd0f4bdc2"",
                      ""CfdiDriverVersion"": ""4.0"",
                      ""IdDeliveries"": [ 10, 11, 12 ],
                      ""IdSapOrders"": [ 555, 556, 557 ],
                      ""CreateUserId"": """",
                      ""DxpOrderId"": ""02573ead-7cae-4654-8ba0-3edbd441904d"",
                      ""InvoiceType"": ""complete"",
                      ""BillingType"": ""complete""
                    }", IsProcessing = false, Status = "Error al crear", IdInvoiceError = 2, ManualChangeApplied = true,
                },
                new ()
                {
                    Id = "28d8520c-c4f7-4c2a-8df5-586adb7c0c94", CreateDate = DateTime.UtcNow.AddMinutes(-1), Payload = @"{""CardCode"": ""C000123"",
                      ""ProcessId"": ""28d8520c-c4f7-4c2a-8df5-586adb7c0c94"",
                      ""CfdiDriverVersion"": ""4.0"",
                      ""IdDeliveries"": [ 10, 11, 12 ],
                      ""IdSapOrders"": [ 555, 556, 557 ],
                      ""CreateUserId"": """",
                      ""DxpOrderId"": ""8f11d8a2-bc17-42cb-998a-86c5040a8f6d"",
                      ""InvoiceType"": ""complete"",
                      ""BillingType"": ""complete""
                    }", IsProcessing = false, Status = "Error al crear", IdInvoiceError = 2, ManualChangeApplied = false,
                },
            };

        /// <summary>
        /// Get GetInvoiceErrorModel.
        /// </summary>
        /// <returns>The GetInvoiceErrorModel.</returns>
        public IEnumerable<InvoiceErrorModel> GetInvoiceErrorModel()
            => new List<InvoiceErrorModel>()
            {
                // RETRY
                new () { Id = 1, Code = "55P03", Error = "lock_not_available – could not obtain lock on row/table because another transaction holds it", ErrorMessage = "Otro proceso está usando esta información. Espera unos segundos e inténtalo de nuevo.", RequireManualChange = false },
                new () { Id = 2, Code = "301", Error = "No matching records found (ODBC -2028)", ErrorMessage = "Uno de los datos enviados (cliente o producto) no existe. Verifica la información.", RequireManualChange = true },
            };
    }
}
