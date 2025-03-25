// <summary>
// <copyright file="AzureAdModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Dtos.Models
{
    /// <summary>
    /// Class azure.
    /// </summary>
    public class AzureAdModel
    {
        /// <summary>
        /// Gets or sets the authentication mode used for integration.
        /// </summary>
        /// <value> Authentication mode (e.g., OAuth, SAML, etc.). </value>
        public string AuthenticationMode { get; set; }

        /// <summary>
        /// Gets or sets the authority URL used in the authentication process.
        /// </summary>
        /// <value> Authorization server URL. </value>
        public string AuthorityUrl { get; set; }

        /// <summary>
        /// Gets or sets the unique client ID registered with the identity provider.
        /// </summary>
        /// <value> Unique client identifier (Client ID). </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the unique tenant ID in the identity provider.
        /// </summary>
        /// <value> Tenant identifier (Tenant ID). </value>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the base scope used in authentication or authorization.
        /// </summary>
        /// <value> Base scope for access requests. </value>
        public string ScopeBase { get; set; }

        /// <summary>
        /// Gets or sets the Power BI username for authentication.
        /// </summary>
        /// <value> Power BI username for authentication. </value>
        public string PbiUsername { get; set; }

        /// <summary>
        /// Gets or sets the password associated with the Power BI username.
        /// </summary>
        /// <value> Power BI user's password. </value>
        public string PbiPassword { get; set; }

        /// <summary>
        /// Gets or sets the client secret used in the authentication process.
        /// </summary>
        /// <value> Client secret for OAuth authentication. </value>
        public string ClientSecret { get; set; }
    }
}
