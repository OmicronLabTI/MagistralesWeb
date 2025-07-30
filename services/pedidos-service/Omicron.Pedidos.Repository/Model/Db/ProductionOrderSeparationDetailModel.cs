// <summary>
// <copyright file="ProductionOrderSeparationDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model.Db
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class productionorderseparationdetail Model.
    /// </summary>
    [Table("productionorderseparationdetail")]
    public class ProductionOrderSeparationDetailModel
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
        /// Gets or sets detailproductionorderid.
        /// </summary>
        /// <value>
        /// Int detailproductionorderid.
        /// </value>
        [Column("detailproductionorderid")]
        public int DetailOrderId { get; set; }

        /// <summary>
        /// Gets or sets OrderId.
        /// </summary>
        /// <value>
        /// Int OrderId.
        /// </value>
        [Column("productionorderid")]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// String userid.
        /// </value>
        [Column("userid")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets createdat.
        /// </summary>
        /// <value>
        /// DateTime createdat.
        /// </value>
        [Column("createdat")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets DxpOrder.
        /// </summary>
        /// <value>
        /// String DxpOrder.
        /// </value>
        [Column("dxporder")]
        public string DxpOrder { get; set; }

        /// <summary>
        /// Gets or sets saporder.
        /// </summary>
        /// <value>
        /// int saporder.
        /// </value>
        [Column("saporder")]
        public int SapOrder { get; set; }

        /// <summary>
        /// Gets or sets assignedpieces .
        /// </summary>
        /// <value>
        /// int assignedpieces .
        /// </value>
        [Column("assignedpieces")]
        public int AssignedPieces { get; set; }

        /// <summary>
        /// Gets or sets consecutiveindex .
        /// </summary>
        /// <value>
        /// int consecutiveindex .
        /// </value>
        [Column("consecutiveindex")]
        public int ConsecutiveIndex { get; set; }
    }
}
