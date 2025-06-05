// <summary>
// <copyright file="OrderFiltersByConfigType.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    using System.Collections.Generic;

    /// <summary>
    /// class for model OrderFiltersByConfigType.
    /// </summary>
    public class OrderFiltersByConfigType
    {
        /// <summary>
        /// Gets or sets ClassificationCodes.
        /// </summary>
        /// <value>The ClassificationCodes.</value>
        public List<string> ClassificationCodes { get; set; }

        /// <summary>
        /// Gets or sets ItemCodesIncludedByConfigRules.
        /// </summary>
        /// <value>The ItemCodesIncludedByConfigRules.</value>
        public List<string> ItemCodesIncludedByConfigRules { get; set; }

        /// <summary>
        /// Gets or sets ItemCodesExcludedByException.
        /// </summary>
        /// <value>The ItemCodesExcludedByException.</value>
        public List<string> ItemCodesExcludedByException { get; set; }

        /// <summary>
        /// Gets or sets InvalidCatalogsGroups.
        /// </summary>
        /// <value>The InvalidCatalogsGroups.</value>
        public List<string> InvalidCatalogsGroups { get; set; }
    }
}
