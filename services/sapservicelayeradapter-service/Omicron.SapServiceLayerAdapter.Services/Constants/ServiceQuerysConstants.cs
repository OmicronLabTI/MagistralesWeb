// <summary>
// <copyright file="ServiceQuerysConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Constants
{
    /// <summary>
    /// class for service querys constants.
    /// </summary>
    public static class ServiceQuerysConstants
    {
        /// <summary>
        /// Query to get the last generated order.
        /// </summary>
        public const string QryGetLastGeneratedOrder = "Orders?$orderby=DocEntry desc&$top=1";

        /// <summary>
        /// Query to get invoice document by doc entry.
        /// </summary>
        public const string QryInvoiceDocumentByDocEntry = "Invoices({0})";

        /// <summary>
        /// Query to get invoice document by doc entry.
        /// </summary>
        public const string QryGetShippingTypesByName = "ShippingTypes?$filter=Name eq '{0}'";

        /// <summary>
        /// Query to get invoice document by doc entry.
        /// </summary>
        public const string QryOrdersDocumentByDocEntry = "Orders({0})";
    }
}
