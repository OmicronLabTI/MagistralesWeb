// <summary>
// <copyright file="RawMaterialRequestModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model.Db
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class for request raw materia.
    /// </summary>
    [Table("rawmaterialrequest")]
    public class RawMaterialRequestModel
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
        /// Gets or sets the production order id.
        /// </summary>
        /// <value>
        /// Id production id.
        /// </value>
        [Column("productionorderid")]
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Byte[] signature.
        /// </value>
        [Column("signature")]
        public byte[] Signature { get; set; }

        /// <summary>
        /// Gets or sets signing user id.
        /// </summary>
        /// <value>
        /// String user id.
        /// </value>
        [Column("signinguserid")]
        public string SigningUserId { get; set; }

        /// <summary>
        /// Gets or sets observations.
        /// </summary>
        /// <value>
        /// String observations.
        /// </value>
        [Column("observations")]
        public string Observations { get; set; }

        /// <summary>
        /// Gets or sets ordered products.
        /// </summary>
        /// <value>
        /// List products.
        /// </value>
        [NotMapped]
        public List<RawMaterialRequestDetailModel> OrderedProducts { get; set; }

        /// <summary>
        /// Gets or sets creation user id.
        /// </summary>
        /// <value>
        /// Int user id.
        /// </value>
        [Column("creationuserid")]
        public string CreationUserId { get; set; }

        /// <summary>
        /// Gets or sets creation date.
        /// </summary>
        /// <value>
        /// String creation date.
        /// </value>
        [Column("creationdate")]
        public string CreationDate { get; set; }
    }
}
