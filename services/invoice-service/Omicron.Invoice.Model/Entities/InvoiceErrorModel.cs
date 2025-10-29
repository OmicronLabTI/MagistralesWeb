// <summary>
// <copyright file="InvoiceErrorModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Model.Entities
{
    /// <summary>
    /// InvoiceErrorModel class.
    /// </summary>
    public class InvoiceErrorModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceErrorModel"/> class.
        /// </summary>
        public InvoiceErrorModel()
        {
            Invoices = new HashSet<InvoiceModel>();
        }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error name.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether manual change is required.
        /// </summary>
        public bool RequireManualChange { get; set; }

        /// <summary>
        /// Gets or sets navigation property for related invoices.
        /// </summary>
        public ICollection<InvoiceModel> Invoices { get; set; }
    }
}
