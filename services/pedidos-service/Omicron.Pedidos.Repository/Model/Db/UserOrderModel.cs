// <summary>
// <copyright file="UserOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Omicron.Pedidos.Resources.Enums;

    /// <summary>
    /// Class OrderLog Model.
    /// </summary>
    [Table("usersorders")]
    public class UserOrderModel
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
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        [Column("userid")]
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets salesorderid.
        /// </summary>
        /// <value>
        /// String salesorderid.
        /// </value>
        [Column("salesorderid")]
        public string Salesorderid { get; set; }

        /// <summary>
        /// Gets or sets productionorderid.
        /// </summary>
        /// <value>
        /// String productionorderid.
        [Column("productionorderid")]
        public string Productionorderid { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        [Column("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets comments.
        /// </summary>
        /// <value>
        /// String comments.
        [Column("comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        [Column("finishdate")]
        public string FinishDate { get; set; }

        /// <summary>
        /// Gets or sets creation date.
        /// </summary>
        /// <value>
        /// String creation date.
        [Column("creationdate")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Gets or sets creator user id.
        /// </summary>
        /// <value>
        /// String creator user id.
        [Column("creationuserid")]
        public string CreatorUserId { get; set; }

        /// <summary>
        /// Gets or sets close date.
        /// </summary>
        /// <value>
        /// String close date.
        [Column("closedate")]
        public string CloseDate { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("closeuserid")]
        public string CloseUserId { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("finishedlabel")]
        public int FinishedLabel { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("qrmgestrcuture")]
        public string MagistralQr { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("finalizeddate")]
        public DateTime? FinalizedDate { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("statusalmacen")]
        public string StatusAlmacen { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("usercheckin")]
        public string UserCheckIn { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("datecheckin")]
        public DateTime? DateTimeCheckIn { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("qrremisionmgestrcuture")]
        public string RemisionQr { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        [Column("deliveryid")]
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is isolated production order.
        [NotMapped]
        public bool IsIsolatedProductionOrder => string.IsNullOrEmpty(this.Salesorderid);

        /// <summary>
        /// Gets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is sales order.
        [NotMapped]
        public bool IsSalesOrder => string.IsNullOrEmpty(this.Productionorderid);

        /// <summary>
        /// Gets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is production order.
        [NotMapped]
        public bool IsProductionOrder => !string.IsNullOrEmpty(this.Productionorderid);

        /// <summary>
        /// Gets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// the value for the status.
        public int StatusOrder => !string.IsNullOrEmpty(this.Status) && this.IsProductionOrder ? (int)Enum.Parse(typeof(StatusEnum), this.Status) : 0;
    }
}