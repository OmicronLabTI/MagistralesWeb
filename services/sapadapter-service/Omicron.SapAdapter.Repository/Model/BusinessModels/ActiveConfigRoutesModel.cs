// <summary>
// <copyright file="ActiveConfigRoutesModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    /// <summary>
    /// Class sorting route.
    /// </summary>
    public class ActiveConfigRoutesModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Id. </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value classification.
        /// </summary>
        /// <value> Classification. </value>
        public string Classification { get; set; }

        /// <summary>
        /// Gets or sets a value classification.
        /// </summary>
        /// <value> Classification. </value>
        public string ClassificationCode { get; set; }

        /// <summary>
        /// Gets or sets a value exceptions.
        /// </summary>
        /// <value> Exceptions. </value>
        public string Exceptions { get; set; }

        /// <summary>
        /// Gets or sets a value itemcode.
        /// </summary>
        /// <value> ItemCode. </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets a value color.
        /// </summary>
        /// <value> Color. </value>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets a value route.
        /// </summary>
        /// <value> Route. </value>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value isactive.
        /// </summary>
        /// <value> Status. </value>
        public bool IsActive { get; set; }
    }
}
