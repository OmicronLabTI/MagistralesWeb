// <summary>
// <copyright file="RawMaterialRequestOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Entities.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class for request raw materia.
    /// </summary>
    [Table("rawmaterialrequestorders")]
    public class RawMaterialRequestOrderModel
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
        /// Gets or sets the request id.
        /// </summary>
        /// <value>
        /// Request id.
        /// </value>
        [Column("rawmaterialrequestid")]
        public int RequestId { get; set; }

        /// <summary>
        /// Gets or sets production order id.
        /// </summary>
        /// <value>The production order id.</value>
        [Column("productionorderid")]
        public int ProductionOrderId { get; set; }
    }
}
