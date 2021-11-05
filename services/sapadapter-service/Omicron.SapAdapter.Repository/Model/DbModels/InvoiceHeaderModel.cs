// <summary>
// <copyright file="InvoiceHeaderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class for the invoice header.
    /// </summary>
    [Table("OINV")]
    public class InvoiceHeaderModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Key]
        [Column("DocEntry")]
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("DocNum")]
        public int DocNum { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("CardName")]
        public string Medico { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("DocDate")]
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("DocStatus")]
        public string InvoiceStatus { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("Address2")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("Comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("CardCode")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("ShipToCode")]
        public string ShippingAddressName { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("U_Comentarios")]
        public string CommentsInvoice { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("TrackNo")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("TrnspCode")]
        public short TransportCode { get; set; }

        /// <summary>
        /// Gets or sets SalesPrsonId.
        /// </summary>
        /// <value>The code.</value>
        [Column("SlpCode")]
        public int SalesPrsonId { get; set; }

        /// <summary>
        /// Gets or sets SalesPrsonId.
        /// </summary>
        /// <value>The code.</value>
        [Column("U_TipoPedido")]
        public string TypeOrder { get; set; }

        /// <summary>
        /// Gets or sets SalesPrsonId.
        /// </summary>
        /// <value>The code.</value>
        [Column("UpdateDate")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("CANCELED")]
        public string Canceled { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("U_Refactura")]
        public string Refactura { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public string ClientEmail { get; set; }

        /// <summary>
        /// Gets or sets a value sales person email.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public string SalesPrsonEmail { get; set; }

        /// <summary>
        /// Gets or sets a value person names.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public string SalesPrsonName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public string TransportName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public string SaleOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public int TotalSaleOrder { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [NotMapped]
        public string Cliente { get; set; }
    }
}
