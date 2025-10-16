// <summary>
// <copyright file="AlmacenSalesHeaderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;

    /// <summary>
    /// Gets the almacen sales header.
    /// </summary>
    public class AlmacenSalesHeaderModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DocNum { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Doctor { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string TypeSaleOrder { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int RemissionedItems { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal TotalPieces { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal RemissionedPieces { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime InitDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string OrderCounter { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Remision { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string InvoiceType { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string TypeOrder { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime? DataCheckin { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string OrderMuestra { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string SapComments { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ListSaleOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has invoice.
        /// </summary>
        /// <value>The code.</value>
        public bool HasInvoice { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string StoredBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsPackage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsFromDxpId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DxpId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsOmigenomics { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int RemittedPieces { get; set; }

        /// <summary>
        /// Gets OrderList.
        /// </summary>
        /// <value>The OrderList.</value>
        public string OrderList => this.ListSaleOrder;

        /// <summary>
        /// Gets RemissionList.
        /// </summary>
        /// <value>The RemissionList.</value>
        public string RemissionList => this.Remision.ToString();

        /// <summary>
        /// Gets or sets a value indicating whether has invoice.
        /// </summary>
        /// <value>The code.</value>
        public bool HasChildOrders { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string BillingType { get; set; }
    }
}
