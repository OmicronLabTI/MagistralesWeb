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
