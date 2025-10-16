// <summary>
// <copyright file="CountDxpOrdersDetail.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.JoinsModels
{
    /// <summary>
    /// Class for the order model.
    /// </summary>
    public class CountDxpOrdersDetail
    {
        /// <summary>
        /// Gets or sets OrderDxpId.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets OrderDxpId.
        /// </summary>
        /// <value>The IsChecked.</value>
        public int DocNum { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CatalogGroup { get; set; }

        /// <summary>
        /// Gets or sets ProductFirmName.
        /// </summary>
        /// <value>
        /// string ProductFirmName.
        /// </value>
        public string ProductFirmName { get; set; }

        /// <summary>
        /// Gets or sets Pieces.
        /// </summary>
        /// <value>
        /// string Pieces.
        /// </value>
        public int Pieces { get; set; }
    }
}
