// <summary>
// <copyright file="InstitutionalClientNamesModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    /// <summary>
    /// Class institutional client names.
    /// </summary>
    public class InstitutionalClientNamesModel
    {
        /// <summary>
        /// Gets or sets Card Code.
        /// </summary>
        /// <value> Card Code. </value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets DocNum.
        /// </summary>
        /// <value> DocNum. </value>
        public int DocNum { get; set; }

        /// <summary>
        /// Gets or sets NameDoctor.
        /// </summary>
        /// <value> NameDoctor. </value>
        public string NameDoctor { get; set; }

        /// <summary>
        /// Gets or sets NameClient.
        /// </summary>
        /// <value> NameClient. </value>
        public string NameClient { get; set; }

        /// <summary>
        /// Gets or sets client type.
        /// </summary>
        /// <value> client type. </value>
        public string ClientType { get; set; }
    }
}
