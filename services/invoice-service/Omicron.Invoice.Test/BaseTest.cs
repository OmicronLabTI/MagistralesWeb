// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Test
{
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
            };

        /// <summary>
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
        /// Get UsersResponse.
        /// </summary>
        /// <returns>The UsersResponse.</returns>
        public ResultDto GetUsersResponse()
        {
            var listUsers = new List<UserModel>
            {
                new UserModel { Activo = 1, FirstName = "Victor", Id = "168783d1-9892-4318-8e3e-03f98d3bf384", LastName = "Sanchez", Password = "123", Role = 5, UserName = "UsuarioAlmacen", Piezas = 1000, Asignable = 1, },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listUsers),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<InvoiceModel> GetAllInvoices()
            => new List<InvoiceModel>()
            {
                new InvoiceModel { Id = "INV-001", DxpOrderId = "DXP-1001", IdInvoiceError = 1, CreateDate = DateTime.UtcNow.AddDays(-10), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Error al crear", IdFacturaSap = null, TypeInvoice = "Gen�rica", BillingType = "Completa", InvoiceCreateDate = null, ErrorMessage = "Error al conectar con SAP", UpdateDate = DateTime.UtcNow.AddDays(-8), RetryNumber = 2, Type = "Autom�tica", ManualChangeApplied = null, IsProcessing = false, Payload = "{}" },
                new InvoiceModel { Id = "INV-002", DxpOrderId = "DXP-1002", IdInvoiceError = null, CreateDate = DateTime.UtcNow.AddDays(-9), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Enviada a crear", IdFacturaSap = null, TypeInvoice = "No Gen�rica", BillingType = "Parcial", InvoiceCreateDate = null, ErrorMessage = null, UpdateDate = null, RetryNumber = 0, Type = "Autom�tica", ManualChangeApplied = null, IsProcessing = true, Payload = "{}" },
                new InvoiceModel { Id = "INV-003", DxpOrderId = "DXP-1003", IdInvoiceError = 2, CreateDate = DateTime.UtcNow.AddDays(-8), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Creando factura", IdFacturaSap = null, TypeInvoice = "Gen�rica", BillingType = "Completa", InvoiceCreateDate = null, ErrorMessage = "Timeout SAP", UpdateDate = DateTime.UtcNow.AddDays(-7), RetryNumber = 1, Type = "Autom�tica", ManualChangeApplied = false, IsProcessing = true, Payload = "{}" },
                new InvoiceModel { Id = "INV-004", DxpOrderId = "DXP-1004", IdInvoiceError = null, CreateDate = DateTime.UtcNow.AddDays(-7), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Enviada a crear", IdFacturaSap = 2001, TypeInvoice = "No Gen�rica", BillingType = "Completa", InvoiceCreateDate = DateTime.UtcNow.AddDays(-6), ErrorMessage = null, UpdateDate = DateTime.UtcNow.AddDays(-5), RetryNumber = 0, Type = "Manual", ManualChangeApplied = true, IsProcessing = false, Payload = "{}" },
                new InvoiceModel { Id = "INV-005", DxpOrderId = "DXP-1005", IdInvoiceError = 3, CreateDate = DateTime.UtcNow.AddDays(-6), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Error al crear", IdFacturaSap = null, TypeInvoice = "Gen�rica", BillingType = "Parcial", InvoiceCreateDate = null, ErrorMessage = "Datos incompletos en remisi�n", UpdateDate = DateTime.UtcNow.AddDays(-5), RetryNumber = 3, Type = "Autom�tica", ManualChangeApplied = false, IsProcessing = false, Payload = "{}" },
                new InvoiceModel { Id = "INV-006", DxpOrderId = "DXP-1006", IdInvoiceError = null, CreateDate = DateTime.UtcNow.AddDays(-5), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Creando factura", IdFacturaSap = null, TypeInvoice = "No Gen�rica", BillingType = "Completa", InvoiceCreateDate = null, ErrorMessage = null, UpdateDate = null, RetryNumber = 0, Type = "Autom�tica", ManualChangeApplied = false, IsProcessing = true, Payload = "{}" },
                new InvoiceModel { Id = "INV-007", DxpOrderId = "DXP-1007", IdInvoiceError = null, CreateDate = DateTime.UtcNow.AddDays(-4), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Enviada a crear", IdFacturaSap = null, TypeInvoice = "Gen�rica", BillingType = "Completa", InvoiceCreateDate = null, ErrorMessage = null, UpdateDate = null, RetryNumber = 1, Type = "Autom�tica", ManualChangeApplied = false, IsProcessing = false, Payload = "{}" },
                new InvoiceModel { Id = "INV-008", DxpOrderId = "DXP-1008", IdInvoiceError = null, CreateDate = DateTime.UtcNow.AddDays(-3), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Creando factura", IdFacturaSap = null, TypeInvoice = "No Gen�rica", BillingType = "Parcial", InvoiceCreateDate = null, ErrorMessage = null, UpdateDate = null, RetryNumber = 2, Type = "Manual", ManualChangeApplied = true, IsProcessing = true, Payload = "{}" },
                new InvoiceModel { Id = "INV-009", DxpOrderId = "DXP-1009", IdInvoiceError = 4, CreateDate = DateTime.UtcNow.AddDays(-2), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Error al crear", IdFacturaSap = null, TypeInvoice = "Gen�rica", BillingType = "Completa", InvoiceCreateDate = null, ErrorMessage = "Error de validaci�n de datos", UpdateDate = DateTime.UtcNow.AddDays(-1), RetryNumber = 1, Type = "Autom�tica", ManualChangeApplied = false, IsProcessing = false, Payload = "{}" },
                new InvoiceModel { Id = "INV-010", DxpOrderId = "DXP-1010", IdInvoiceError = null, CreateDate = DateTime.UtcNow.AddDays(-1), AlmacenUser = "168783d1-9892-4318-8e3e-03f98d3bf384", Status = "Enviada a crear", IdFacturaSap = null, TypeInvoice = "No Gen�rica", BillingType = "Completa", InvoiceCreateDate = null, ErrorMessage = null, UpdateDate = null, RetryNumber = 0, Type = "Autom�tica", ManualChangeApplied = false, IsProcessing = false, Payload = "{}" },
                new InvoiceModel { Id = "1", DxpOrderId = "XXXX", AlmacenUser = "Test", CreateDate = DateTime.Now, Status = ServiceConstants.SendToCreateInvoice, RetryNumber = 0, IsProcessing = true, Payload = "{}" },
                new InvoiceModel { Id = "2", DxpOrderId = "XXXX", AlmacenUser = "Test", CreateDate = DateTime.Now, Status = ServiceConstants.SendToCreateInvoice, RetryNumber = 0, IsProcessing = false, Payload = "{}" },
            };

        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<InvoiceErrorModel> GetAllErrors()
            => new List<InvoiceErrorModel>()
            {
                new InvoiceErrorModel { Id = 1, Code = "ERR001", Error = "Error SAP", ErrorMessage = "No se pudo crear factura en SAP", RequireManualChange = false },
                new InvoiceErrorModel { Id = 2, Code = "ERR002", Error = "Timeout", ErrorMessage = "El servicio SAP no respondio", RequireManualChange = false },
                new InvoiceErrorModel { Id = 3, Code = "ERR003", Error = "Datos incompletos", ErrorMessage = "Faltan datos obligatorios en remision", RequireManualChange = true },
                new InvoiceErrorModel { Id = 4, Code = "ERR004", Error = "Validacion", ErrorMessage = "Datos invalidos en factura", RequireManualChange = false },
                new InvoiceErrorModel { Id = 5, Code = "C01", Error = "Error", ErrorMessage = "Error", RequireManualChange = true },
            };

        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<InvoiceRemissionModel> GetAllRemissions()
            => new List<InvoiceRemissionModel>()
            {
                new InvoiceRemissionModel { Id = 1, RemissionId = 101, IdInvoice = "INV-001" },
                new InvoiceRemissionModel { Id = 2, RemissionId = 102, IdInvoice = "INV-001" },
                new InvoiceRemissionModel { Id = 3, RemissionId = 103, IdInvoice = "INV-002" },
                new InvoiceRemissionModel { Id = 4, RemissionId = 104, IdInvoice = "INV-003" },
                new InvoiceRemissionModel { Id = 5, RemissionId = 105, IdInvoice = "INV-004" },
                new InvoiceRemissionModel { Id = 6, RemissionId = 106, IdInvoice = "INV-005" },
                new InvoiceRemissionModel { Id = 7, RemissionId = 107, IdInvoice = "INV-006" },
                new InvoiceRemissionModel { Id = 8, RemissionId = 108, IdInvoice = "INV-006" },
                new InvoiceRemissionModel { Id = 9, RemissionId = 109, IdInvoice = "INV-008" },
                new InvoiceRemissionModel { Id = 10, RemissionId = 110, IdInvoice = "INV-010" },
                new InvoiceRemissionModel { Id = 11, IdInvoice = "1", RemissionId = 1 },
            };

        /// <summary>
        /// Get GetInvoiceSapOrderModel.
        /// </summary>
        /// <returns>The GetInvoiceSapOrderModel.</returns>
        public List<InvoiceSapOrderModel> GetInvoiceSapOrderModel()
            => new List<InvoiceSapOrderModel>()
            {
                new InvoiceSapOrderModel { Id = 1, SapOrderId = 5001, IdInvoice = "INV-001" },
                new InvoiceSapOrderModel { Id = 2, SapOrderId = 5002, IdInvoice = "INV-003" },
                new InvoiceSapOrderModel { Id = 3, SapOrderId = 5003, IdInvoice = "INV-003" },
                new InvoiceSapOrderModel { Id = 4, SapOrderId = 5004, IdInvoice = "INV-004" },
                new InvoiceSapOrderModel { Id = 5, SapOrderId = 5005, IdInvoice = "INV-007" },
                new InvoiceSapOrderModel { Id = 6, SapOrderId = 5006, IdInvoice = "INV-008" },
                new InvoiceSapOrderModel { Id = 7, SapOrderId = 5007, IdInvoice = "INV-008" },
            };
    }
}
