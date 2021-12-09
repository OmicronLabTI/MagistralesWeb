// <summary>
// <copyright file="CompleteInvoiceDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.JoinsModels
{
    using Omicron.SapAdapter.Entities.Model.DbModels;

    /// <summary>
    /// the clas for Complete Invoice Detail Model.
    /// </summary>
    public class CompleteInvoiceDetailModel
    {
        /// <summary>
        /// Gets or sets the invoice header.
        /// </summary>
        /// <value>
        /// the invoice header.
        /// </value>
        public InvoiceHeaderModel InvoiceHeader { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// the client.
        /// </value>
        public string Cliente { get; set; }

        /// <summary>
        /// Gets or sets the medico.
        /// </summary>
        /// <value>
        /// the medico.
        /// </value>
        public string Medico { get; set; }

        /// <summary>
        /// Gets or sets the invoice detail.
        /// </summary>
        /// <value>
        /// the invoice detail.
        /// </value>
        public InvoiceDetailModel Detail { get; set; }
    }
}
