// <summary>
// <copyright file="IncidentInfoModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System.Collections.Generic;

    /// <summary>
    /// class for the info model.
    /// </summary>
    public class IncidentInfoModel
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Incidence { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public List<AlmacenBatchModel> Batches { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Status { get; set; }
    }
}
