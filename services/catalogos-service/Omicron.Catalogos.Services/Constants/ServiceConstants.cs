// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Constants
{
    /// <summary>
    /// Service constants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Constant for Offset.
        /// </summary>
        public const string ErrorDownload = "Formulas - Microservice Catalogos - Error to download azure blob {0} Error: {1} --- {2}";

        /// <summary>
        /// azure account.
        /// </summary>
        public const string AzureAccountName = "AzureAccountName";

        /// <summary>
        /// azure account.
        /// </summary>
        public const string AzureAccountKey = "AzureAccountKey";

        /// <summary>
        /// the container file.
        /// </summary>
        public const string WarehousesFileUrl = "WarehousesFileUrl";

        /// <summary>
        /// Is active.
        /// </summary>
        public const string IsActive = "activo";

        /// <summary>
        /// Constant for Limit.
        /// </summary>
        public const string Warehouses = "warehouses";

        /// <summary>
        /// NoMatching.
        /// </summary>
        public const string NoMatching = "Los siguientes almacenes {0} tienen una configuración incorrecta: no están registrados en SAP, presentan duplicidad en su registro o tienen fabricantes, productos o catálogos asociados inexistentes.\r\n ";

        /// <summary>
        /// AzureAdEnvDataKey.
        /// </summary>
        public const string AzureAdEnvDataKey = "AzureAd";

        /// <summary>
        /// AzureAdEnvDataKey.
        /// </summary>
        public const string Manufacturers = "manufacturers/all";

        /// <summary>
        /// AzureAdEnvDataKey.
        /// </summary>
        public const string Products = "products";
    }
}
