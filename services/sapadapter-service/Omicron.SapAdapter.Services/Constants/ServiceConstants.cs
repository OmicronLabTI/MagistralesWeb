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
        /// The type value.
        /// </summary>
        public const string Type = "type";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Label = "label";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FinishedLabel = "finlabel";

        /// <summary>
        /// The order type dict.
        /// </summary>
        public const string OrderType = "ordtype";

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
        /// the filter for orders.
        /// </summary>
        public const string Current = "current";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Advance = "advance";

        /// <summary>
        /// the doctor filter.
        /// </summary>
        public const string Doctor = "doctor";

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
        /// en proceso to show.
        /// </summary>
        public const string Asignado = "Asignado";

        /// <summary>
        /// The enviado status.
        /// </summary>
        public const string Camino = "En Camino";

        /// <summary>
        /// route to get the users sales orders.
        /// </summary>
        public const string GetUserSalesOrder = "getUserOrder/salesOrder";

        /// <summary>
        /// route to get the user fab order.
        /// </summary>
        public const string GetUserOrders = "getUserOrder/fabOrder";

        /// <summary>
        /// route to get the user fab order.
        /// </summary>
        public const string GetOrdersByStatusAndUserId = "qfbOrders/{0}/{1}";

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
        /// Get the lines products for status almacenado.
        /// </summary>
        public const string GetLineProductPedidos = "almacen/orders";

        /// <summary>
        /// route for the delivery values.
        /// </summary>
        public const string GetLinesForDelivery = "delivery/orders";

        /// <summary>
        /// Get the delivery orders.
        /// </summary>
        public const string GetUserOrderDelivery = "userorders/delivery";

        /// <summary>
        /// Get the user orders for invoices.
        /// </summary>
        public const string GetUserOrderInvoice = "userorders/invoice";

        /// <summary>
        /// Value  for the lines for invoice.
        /// </summary>
        public const string GetLinesForInvoice = "invoice/orders";

        /// <summary>
        /// Value  for the lines for invoice.
        /// </summary>
        public const string GetLinesBySaleOrder = "getline/saleorder";

        /// <summary>
        /// Value  for the lines for invoice.
        /// </summary>
        public const string GetPackagesByInvoice = "getpackages/invoice";

        /// <summary>
        /// gets the incidents by sale id.
        /// </summary>
        public const string GetIncidents = "incident/saleorder";

        /// <summary>
        /// gets the ids for lookup.
        /// </summary>
        public const string AdvanceLookId = "advance/id/look";

        /// <summary>
        /// route to look for user orders.
        /// </summary>
        public const string GetOrderByQuery = "userorders";

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
        /// Status finalizado.
        /// </summary>
        public const string Liberado = "Liberado";

        /// <summary>
        /// status por recibir.
        /// </summary>
        public const string PorRecibir = "Por recibir";

        /// <summary>
        /// status almacenado.
        /// </summary>
        public const string Almacenado = "Almacenado";

        /// <summary>
        /// the empaquetado status.
        /// </summary>
        public const string Empaquetado = "Empaquetado";

        /// <summary>
        /// status not delivered.
        /// </summary>
        public const string NoEntregado = "No Entregado";

        /// <summary>
        /// The enviado status.
        /// </summary>
        public const string Entregado = "Entregado";

        /// <summary>
        /// The enviado status.
        /// </summary>
        public const string Enviado = "Enviado";

        /// <summary>
        /// status pendiente.
        /// </summary>
        public const string Pendiente = "Pendiente";

        /// <summary>
        /// status back order.
        /// </summary>
        public const string BackOrder = "Back Order";

        /// <summary>
        /// Status recibir.
        /// </summary>
        public const string Recibir = "Recibir";

        /// <summary>
        /// Status recibir.
        /// </summary>
        public const string Cancelado = "Cancelado";

        /// <summary>
        /// Magistral.
        /// </summary>
        public const string Magistral = "Magistral";

        /// <summary>
        /// producto de linea.
        /// </summary>
        public const string Linea = "de Línea";

        /// <summary>
        /// producto de linea.
        /// </summary>
        public const string Line = "linea";

        /// <summary>
        /// producto mixto.
        /// </summary>
        public const string Mixto = "Mixto";

        /// <summary>
        /// PT wharegouse.
        /// </summary>
        public const string PT = "PT";

        /// <summary>
        /// the nvo leon state.
        /// </summary>
        public const string NuevoLeon = "Nuevo León";

        /// <summary>
        /// the foreign value.
        /// </summary>
        public const string Foraneo = "Foráneo";

        /// <summary>
        /// the local status.
        /// </summary>
        public const string Local = "Local";

        /// <summary>
        /// foregin package error.
        /// </summary>
        public const string ForeingPackage = "El paquete es foráneo y no puede ser entregado por un repartidor local";

        /// <summary>
        /// the packages is not available.
        /// </summary>
        public const string PackageNotAvailable = "El paquete se encuentra";

        /// <summary>
        /// all types.
        /// </summary>
        public const string AllTypes = "magistral,mixto,linea,maquila";

        /// <summary>
        /// all types.
        /// </summary>
        public const string AllStatus = "Recibir,Pendiente,Back Order";

        /// <summary>
        /// Get the params.
        /// </summary>
        public const string GetParams = "params/contains/field";

        /// <summary>
        /// the max days.
        /// </summary>
        public const string SentMaxDays = "AlmacenMaxDayToLook";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string Pedido = "ped";

        /// <summary>
        /// Orden value.
        /// </summary>
        public const string Orden = "ord";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string Delivery = "delivery";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string Invoice = "invoice";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string SaleOrder = "pedido";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RedisComponents = "redisComponents";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RemisionChip = "rem-";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string OrderTypeMQ = "MQ";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Maquila = "Maquila";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string DontExistsTable = "DontExistsTable";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> TypesForUserOrder { get; } = new List<string>
        {
            Magistral.ToLower(),
            Mixto.ToLower(),
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> TypesForLine { get; } = new List<string>
        {
            "linea",
        };

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

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> KeysToIgnoreRedis { get; } = new List<string>
        {
            Offset,
            Limit,
            Advance,
            Current,
            NeedsLargeDsc,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusToIgnoreLineProducts { get; } = new List<string>
        {
            Almacenado,
            Cancelado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusToIgnorePorRecibir { get; } = new List<string>
        {
            Almacenado,
            BackOrder,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusReceptionOrders { get; } = new List<string>
        {
            Finalizado,
            Liberado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusAlmacenReceptionOrders { get; } = new List<string>
        {
            BackOrder,
            Recibir,
        };
    }
}
