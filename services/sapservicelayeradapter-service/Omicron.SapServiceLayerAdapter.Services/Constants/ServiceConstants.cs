// <summary>
// <copyright file="ServiceContants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Constants
{
    /// <summary>
    /// class for cosntants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// The service layer user env.
        /// </summary>
        public const string SAPServiceLayerUserEnvName = "SAPServiceLayer:User";

        /// <summary>
        /// The service layer password env.
        /// </summary>
        public const string SAPServiceLayerPasswordEnvName = "SAPServiceLayer:Pwd";

        /// <summary>
        /// The service layer companyDb env.
        /// </summary>
        public const string SAPServiceLayerDatabaseName = "SAPServiceLayer:CompanyDb";
    }
}
