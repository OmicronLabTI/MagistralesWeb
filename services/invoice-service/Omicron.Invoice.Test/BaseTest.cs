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
    using Newtonsoft.Json;
    using Omicron.Invoice.Services.Constants;

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
                new ()
                {
                    Id = "283ba6a5-e3c1-4810-9c88-a7e66053cf77", CreateDate = DateTime.UtcNow.AddMinutes(-1), Payload = @"{""CardCode"": ""C000123"",
                      ""ProcessId"": ""283ba6a5-e3c1-4810-9c88-a7e66053cf77"",
                      ""CfdiDriverVersion"": ""4.0"",
                      ""IdDeliveries"": [ 10, 11, 12 ],
                      ""IdSapOrders"": [ 555, 556, 557 ],
                      ""CreateUserId"": """",
                      ""DxpOrderId"": ""23a0c066-1360-476f-a921-216a009cdd4"",
                      ""InvoiceType"": ""complete"",
                      ""BillingType"": ""complete""
                    }", IsProcessing = false, Status = "Error al crear", IdInvoiceError = 2, ManualChangeApplied = false, IdFacturaSap = 1234,
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
                new () { Id = 1, Code = "55P03", Error = "lock_not_available � could not obtain lock on row/table because another transaction holds it", ErrorMessage = "Otro proceso est� usando esta informaci�n. Espera unos segundos e int�ntalo de nuevo.", RequireManualChange = false },
                new () { Id = 2, Code = "301", Error = "No matching records found (ODBC -2028)", ErrorMessage = "Uno de los datos enviados (cliente o producto) no existe. Verifica la informaci�n.", RequireManualChange = true },
        /// Creates a result.
        /// </summary>
        /// <param name="response"> the object to send. </param>
        /// <returns> data. </returns>
        public static ResultDto GetResultModel(object response)
        {
            return new ResultDto
            {
                Code = 200,
                Response = JsonConvert.SerializeObject(response),
                Success = true,
            };
        }

        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<InvoiceModel> GetAllInvoices()
            => new List<InvoiceModel>()
            {
                new InvoiceModel { Id = "1", DxpOrderId = "XXXX", AlmacenUser = "Test", CreateDate = DateTime.Now, Status = ServiceConstants.SendToCreateInvoice, RetryNumber = 0, IsProcessing = true, Payload = string.Empty },
                new InvoiceModel { Id = "2", DxpOrderId = "XXXX", AlmacenUser = "Test", CreateDate = DateTime.Now, Status = ServiceConstants.SendToCreateInvoice, RetryNumber = 0, IsProcessing = false, Payload = string.Empty },
            };

        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<InvoiceErrorModel> GetAllErrors()
            => new List<InvoiceErrorModel>()
            {
                new InvoiceErrorModel { Id = 1, Code = "C01", Error = "Error", ErrorMessage = "Error", RequireManualChange = true },
            };

        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<InvoiceRemissionModel> GetAllRemissions()
            => new List<InvoiceRemissionModel>()
            {
                new InvoiceRemissionModel { Id = 1, IdInvoice = "1", RemissionId = 1 },
            };
    }
}
