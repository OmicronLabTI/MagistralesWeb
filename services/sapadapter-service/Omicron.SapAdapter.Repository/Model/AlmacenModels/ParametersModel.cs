// <summary>
// <copyright file="ParametersModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    /// <summary>
    /// class for parameters.
    /// </summary>
    public class ParametersModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Parameter id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>
        /// Parameter value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets parameter name.
        /// </summary>
        /// <value>
        /// Parameter name.
        /// </value>
        public string Field { get; set; }
    }
}
