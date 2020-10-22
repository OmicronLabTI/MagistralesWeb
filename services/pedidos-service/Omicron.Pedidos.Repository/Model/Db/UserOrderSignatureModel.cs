// <summary>
// <copyright file="UserOrderSignatureModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class user order signatures model.
    /// </summary>
    [Table("userorderssignatures")]
    public class UserOrderSignatureModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user order id.
        /// </summary>
        /// <value>
        /// Related user order id.
        /// </value>
        [Column("userorderid")]
        public int UserOrderId { get; set; }

        /// <summary>
        /// Gets or sets technical signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Technical signature.
        /// </value>
        [Column("technicalsignature")]
        public byte[] TechnicalSignature { get; set; }

        /// <summary>
        /// Gets or sets logistics signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Logistics signature.
        [Column("logisticsignature")]
        public byte[] LogisticSignature { get; set; }

        /// <summary>
        /// Gets or sets logistics signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Logistics signature.
        [Column("qfbsignature")]
        public byte[] QfbSignature { get; set; }

        /// <summary>
        /// Gets or sets logistics signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Logistics signature.
        [Column("designersignature")]
        public byte[] DesignerSignature { get; set; }

        /// <summary>
        /// Gets or sets logistics signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Logistics signature.
        [Column("designerid")]
        public string DesignerId { get; set; }
    }
}