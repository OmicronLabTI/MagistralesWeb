// <summary>
// <copyright file="ConfigRoutesModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class sorting route.
    /// </summary>
    [Table("configproductroutesbyclassification")]
    public class ConfigRoutesModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Id. </value>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value classification.
        /// </summary>
        /// <value> Classification. </value>
        [Column("classification")]
        public string Classification { get; set; }

        /// <summary>
        /// Gets or sets a value classification.
        /// </summary>
        /// <value> Classification. </value>
        [Column("classificationcode")]
        public string ClassificationCode { get; set; }

        /// <summary>
        /// Gets or sets a value exceptions.
        /// </summary>
        /// <value> Exceptions. </value>
        [Column("exception")]
        public string Exceptions { get; set; }

        /// <summary>
        /// Gets or sets a value itemcode.
        /// </summary>
        /// <value> ItemCode. </value>
        [Column("itemcode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets a value color.
        /// </summary>
        /// <value> Color. </value>
        [Column("color")]
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets a value route.
        /// </summary>
        /// <value> Route. </value>
        [Column("route")]
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value isactive.
        /// </summary>
        /// <value> Status. </value>
        [Column("isactive")]
        public bool IsActive { get; set; }
    }
}
