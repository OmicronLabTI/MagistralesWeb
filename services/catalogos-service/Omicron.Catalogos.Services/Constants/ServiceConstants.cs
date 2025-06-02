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
        /// the container file.
        /// </summary>
        public const string ManufacturersFileUrl = "ManufacturersFileUrl";

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

        /// <summary>
        /// Error request.
        /// </summary>
        public const string ErrorRequest = "Catalogos - Error peticion catalogs dxp service";

        /// <summary>
        /// the container file.
        /// </summary>
        public const string GetClassificationsByDescription = "classifications";

        /// <summary>
        /// the container file.
        /// </summary>
        public const string HexColor = @"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$";

        /// <summary>
        /// the container file.
        /// </summary>
        public const string RedisKey = "configroute-valids";

        /// <summary>
        /// NoMatching.
        /// </summary>
        public const string InvalidsSortingRoutes = "Los siguientes productos o clasificaciones {0} tienen una configuración incorrecta: no están registrados en SAP, presentan duplicidad en su registro o son inexistentes.";

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>
        /// the order.
        /// </value>
        public static List<string> Exlusions { get; } = new List<string>
        {
            "MX",
            "MQ",
        };
    }
}
