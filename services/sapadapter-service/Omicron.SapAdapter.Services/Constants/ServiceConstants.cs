// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Constants
{
    using System.Collections.Generic;

    /// <summary>
    /// The constants classs.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string DocNum = "docNum";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FechaInicio = "fini";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FechaFin = "ffin";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Status = "status";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Qfb = "qfb";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Label = "label";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FinishedLabel = "finlabel";

        /// <summary>
        /// if needs the large description.
        /// </summary>
        public const string NeedsLargeDsc = "Ldsc";

        /// <summary>
        /// the key for cliente.
        /// </summary>
        public const string Cliente = "cliente";

        /// <summary>
        /// const for offset.
        /// </summary>
        public const string Offset = "offset";

        /// <summary>
        /// Const for the limit.
        /// </summary>
        public const string Limit = "limit";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string ItemCode = "code";

        /// <summary>
        /// the abierto status.
        /// </summary>
        public const string Abierto = "Abierto";

        /// <summary>
        /// value for chips.
        /// </summary>
        public const string Chips = "chips";

        /// <summary>
        /// values cuando no hay chips.
        /// </summary>
        public const string NoChipsError = "No se encontraron filtros";

        /// <summary>
        /// messages when the order doesnt have recipes.
        /// </summary>
        public const string NoRecipes = "No se encontraron recetas para el pedido";

        /// <summary>
        /// get the chips.
        /// </summary>
        public const string ChipSeparator = ",";

        /// <summary>
        /// the proceso status.
        /// </summary>
        public const string Proceso = "Proceso";

        /// <summary>
        /// en proceso to show.
        /// </summary>
        public const string EnProceso = "En proceso";

        /// <summary>
        /// en proceso to show.
        /// </summary>
        public const string Terminado = "Terminado";

        /// <summary>
        /// route to get the users sales orders.
        /// </summary>
        public const string GetUserSalesOrder = "getUserOrder/salesOrder";

        /// <summary>
        /// route to get the user fab order.
        /// </summary>
        public const string GetUserOrders = "getUserOrder/fabOrder";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string GetUsersById = "getUsersById";

        /// <summary>
        /// url for the ones for almacen.
        /// </summary>
        public const string GetUserOrdersAlmancen = "userorders/almacen";

        /// <summary>
        /// Get the lines products for status almacenado.
        /// </summary>
        public const string GetLineProduct = "orders?status=Almacenado";

        /// <summary>
        /// Get the delivery orders.
        /// </summary>
        public const string GetUserOrderDelivery = "userorders/delivery";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string Personalizado = "Personalizada";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string Generico = "Genérica";

        /// <summary>
        /// when stock is missing.
        /// </summary>
        public const string MissingWarehouseStock = "Stock";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string BatchesAreMissingError = "Batches";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string HasRecipe = "si";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string HasNeedsRecipe = "1";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string DoesntHaveNeedRecipe = "2";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string NoNeedRecipe = "3";

        /// <summary>
        /// Status finalizado.
        /// </summary>
        public const string Finalizado = "Finalizado";

        /// <summary>
        /// status por recibir.
        /// </summary>
        public const string PorRecibir = "Por recibir";

        /// <summary>
        /// status almacenado.
        /// </summary>
        public const string Almacenado = "Almacenado";

        /// <summary>
        /// Magistral.
        /// </summary>
        public const string Magistral = "Magistral";

        /// <summary>
        /// producto de linea.
        /// </summary>
        public const string Linea = "de Línea";

        /// <summary>
        /// producto mixto.
        /// </summary>
        public const string Mixto = "Mixto";

        /// <summary>
        /// PT wharegouse.
        /// </summary>
        public const string PT = "PT";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatus { get; } = new Dictionary<string, string>
        {
            { "P", "Planificado" },
            { "L", "Cerrado" },
            { "C", "Cancelado" },
            { "R", "Liberado" },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatusType { get; } = new Dictionary<string, string>
        {
            { "S", "Estandar" },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatusOrigin { get; } = new Dictionary<string, string>
        {
            { "M", "Manual" },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictUrlEncode { get; } = new Dictionary<string, string>
        {
            { "%C3%9C", "Ü" },
            { "%C3%BC", "ü" },
        };
    }
}
