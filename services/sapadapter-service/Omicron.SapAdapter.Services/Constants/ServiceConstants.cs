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
        public const string DocNumDxp = "docNumDxp";

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
        /// Const for the all classifications.
        /// </summary>
        public const string AllClassifications = "Todas";

        /// <summary>
        /// Const for the classifications.
        /// </summary>
        public const string Classifications = "classifications";

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
        /// the doctor filter.
        /// </summary>
        public const string CatalogGroup = "catalogGroup";

        /// <summary>
        /// the abierto status.
        /// </summary>
        public const string Abierto = "Abierto";

        /// <summary>
        /// the abierto status.
        /// </summary>
        public const string AbiertoSap = "O";

        /// <summary>
        /// the datasource di api.
        /// </summary>
        public const string DataSourceDiApi = "O";

        /// <summary>
        /// the datasource di api.
        /// </summary>
        public const string DataSourceServiceLayer = "S";

        /// <summary>
        /// value for chips.
        /// </summary>
        public const string Chips = "chips";

        /// <summary>
        /// value for chips.
        /// </summary>
        public const string Shipping = "shipping";

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
        /// route to get the users sales orders.
        /// </summary>
        public const string GetUserSalesOrderWithDetail = "getUserOrder/salesOrder/detail";

        /// <summary>
        /// route to get the users sales orders.
        /// </summary>
        public const string GetThemes = "products/themes";

        /// <summary>
        /// route to get the user fab order.
        /// </summary>
        public const string GetUserOrders = "getUserOrder/fabOrder";

        /// <summary>
        /// route to get the user fab order.
        /// </summary>
        public const string GetOrdersByStatusAndUserId = "qfbOrders/{0}/{1}";

        /// <summary>
        /// route to get the remitted pieces.
        /// </summary>
        public const string GetRemittedPieces = "infopieces/{0}/{1}";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string GetUsersById = "getUsersById";

        /// <summary>
        /// url for the ones for almacen.
        /// </summary>
        public const string GetUserOrdersAlmancen = "userorders/almacen";

        /// <summary>
        /// url for the ones for almacen.
        /// </summary>
        public const string GetUserOrdersAlmancenId = "userorders/almacen/id";

        /// <summary>
        /// Get the lines products for status almacenado.
        /// </summary>
        public const string GetLineProduct = "orders?status=Almacenado";

        /// <summary>
        /// Get the lines products for status almacenado.
        /// </summary>
        public const string GetLineProductPedidos = "almacen/orders";

        /// <summary>
        /// route to get GetLinesForDeliveryByDatesRange.
        /// </summary>
        public const string GetLinesForDeliveryByDatesRange = "delivery/orders?startdate={0}&enddate={1}";

        /// <summary>
        /// route for the delivery values.
        /// </summary>
        public const string GetLinesForDeliveryId = "delivery/orders/id";

        /// <summary>
        /// Get GetUserOrderDeliveryByDatesRange.
        /// </summary>
        public const string GetUserOrderDeliveryByDatesRange = "userorders/delivery?startdate={0}&enddate={1}";

        /// <summary>
        /// Get the delivery orders.
        /// </summary>
        public const string GetUserOrderDeliveryId = "userorders/delivery/id";

        /// <summary>
        /// Get the delivery orders.
        /// </summary>
        public const string GetUserOrdersByInvoicesIds = "getUserOrder/invoices";

        /// <summary>
        /// Get the delivery orders.
        /// </summary>
        public const string GetUserOrderByDeliveryOrder = "getUserOrder/deliveryOrders";

        /// <summary>
        /// Value  for the lines for invoice.
        /// </summary>
        public const string GetLineOrdersByInvoice = "getline/invoiceId";

        /// <summary>
        /// Get the user orders for invoices.
        /// </summary>
        public const string GetUserOrderInvoiceByRangeDate = "userorders/invoice/byrangedates?startdate={0}&enddate={1}";

        /// <summary>
        /// Value  for the lines for invoice.
        /// </summary>
        public const string GetLinesForInvoiceByRangeDate = "invoice/orders/byrangedates?startdate={0}&enddate={1}";

        /// <summary>
        /// GetUserOrderInvoice.
        /// </summary>
        public const string GetUserOrderInvoice = "userorders/invoice";

        /// <summary>
        /// GetLinesForInvoice.
        /// </summary>
        public const string GetLinesForInvoice = "invoice/orders";

        /// <summary>
        /// GetUserOrderInvoiceByDeliveryIds.
        /// </summary>
        public const string GetUserOrderInvoiceByDeliveryIds = "userorders/invoice/bydeliveryids";

        /// <summary>
        /// GetLinesForInvoiceByDeliveryIds.
        /// </summary>
        public const string GetLinesForInvoiceByDeliveryIds = "invoice/orders/bydeliveryids";

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
        /// the special clients.
        /// </summary>
        public const string SpecialClients = "special/clients";

        /// <summary>
        /// gets the address.
        /// </summary>
        public const string GetDoctorAddress = "doctor/specific/deliveryaddress";

        /// <summary>
        /// gets the address.
        /// </summary>
        public const string GetResponsibleDoctors = "doctor/responsibledoctorinfo";

        /// <summary>
        /// gets the address.
        /// </summary>
        public const string GetAcademicInfoDoctors = "doctor/academicdoctorinfo";

        /// <summary>
        /// gets the address.
        /// </summary>
        public const string GetSpecificDelieryAddress = "doctor/specific/deliveryaddress/list";

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
        public const string PersonalizadoAbr = "P";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string GenericoAbr = "G";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string MixtoAbr = "M";

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
        public const string SignatureAreMissingError = "Signature";

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
        /// Status recibir.
        /// </summary>
        public const string Planificado = "Planificado";

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
        public const string LineaAlone = "Linea";

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
        /// the order Complete.
        /// </summary>
        public const string Complete = "Completa";

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
        public const string AllStatus = "Recibir,Pendiente,Back Order,Cancelado";

        /// <summary>
        /// Get the params.
        /// </summary>
        public const string GetParams = "params/contains/field";

        /// <summary>
        /// the max days.
        /// </summary>
        public const string SentMaxDays = "AlmacenMaxDayToLook";

        /// <summary>
        /// the max days.
        /// </summary>
        public const string LocalNeighborhood = "LocalNeighborhood";

        /// <summary>
        /// the max days.
        /// </summary>
        public const string CardCodeResponsibleMedic = "CardCodeResponsibleMedic";

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
        public const string RedisComponentsInputRequest = "redisComponentsInputRequest";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RedisBulkOrderKey = "redisComponentsBulkOrder";

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
        public const string OrderTypeMU = "MU";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string OrderTypePackage = "PQ";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Maquila = "Maquila";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Muestra = "Muestra";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Paquetes = "Paquetes";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string DontExistsTable = "DontExistsTable";

        /// <summary>
        /// valor for Si.
        /// </summary>
        public const string IsSampleOrder = "Si";

        /// <summary>
        /// valor for No.
        /// </summary>
        public const string IsNotSampleOrder = "No";

        /// <summary>
        /// value for refactua.
        /// </summary>
        public const string IsRefactura = "Si";

        /// <summary>
        /// value for refactua.
        /// </summary>
        public const string MagistralWareHouse = "MG";

        /// <summary>
        /// the line products.
        /// </summary>
        public const string AlmacenLineProducts = "AlmacenLineProducts";

        /// <summary>
        /// the line products.
        /// </summary>
        public const string LocalNeighbors = "LocalNeighbors";

        /// <summary>
        /// if a order is package.
        /// </summary>
        public const string IsPackage = "Y";

        /// <summary>
        /// if a order is package.
        /// </summary>
        public const string IsOmigenomics = "SI";

        /// <summary>
        /// the abierto status.
        /// </summary>
        public const string EndPointToGetPayments = "orders/subtransactions";

        /// <summary>
        /// the nvo leon state.
        /// </summary>
        public const int ShippingCostAccepted = 1;

        /// <summary>
        /// the nvo leon state.
        /// </summary>
        public const string OnSiteDelivery = "Entrega en las instalaciones";

        /// <summary>
        /// the nvo leon state.
        /// </summary>
        public const string WildcardDocNumDxp = "#";

        /// <summary>
        /// if a order is package.
        /// </summary>
        public const string OmigenomicsGroup = "Omigenomics";

        /// <summary>
        /// if a order is package.
        /// </summary>
        public const string WareHouseToExclude = "WareHouseToExclude";

        /// <summary>
        /// if a order is package.
        /// </summary>
        public const string PatientConstant = "Nombre del paciente: ";

        /// <summary>
        /// Digits For Short Shop Transaction.
        /// </summary>
        public const int DigitsForShortShopTransaction = 6;

        /// <summary>
        /// Doctor Address Type.
        /// </summary>
        public const string DoctorAddressType = "doctor";

        /// <summary>
        /// Doctor Address Type.
        /// </summary>
        public const string GetInvalidOrdersByMissingTecnicSign = "get/invalid/productionOrders/byMissingTecnicSign";

        /// <summary>
        /// Reason unexpected error.
        /// </summary>
        public const string OrderWithoutTecnicSign = "No es posible terminar, falta la firma del técnico asignado";

        /// <summary>
        /// Parameter user id.
        /// </summary>
        public const string ParameterUserId = "userId";

        /// <summary>
        /// Qfb user role.
        /// </summary>
        public const int QfbUserRole = 2;

        /// <summary>
        /// The order type dict.
        /// </summary>
        public const string TypeParameter = "type";

        /// <summary>
        /// Detail Order Parameter.
        /// </summary>
        public const string DetailOrderParameter = "detailOrder";

        /// <summary>
        /// Input Request Parameter.
        /// </summary>
        public const string InputRequestParameter = "inputRequest";

        /// <summary>
        /// Bulk Order Parameter.
        /// </summary>
        public const string BulkOrderParameter = "bulkOrder";

        /// <summary>
        /// Bulk Order Parameter.
        /// </summary>
        public const string IdTransaction = "idtransaction";

        /// <summary>
        /// Bulk Order Parameter.
        /// </summary>
        public const string SearchMesssage400 = "Search query is required";

        /// <summary>
        /// gets regex name doctor.
        /// </summary>
        public const string RegexNameDoctor = @"\d+\.\s([A-Z\s]+C\.\d+)";

        /// <summary>
        /// ClientTypeGeneral.
        /// </summary>
        public const string ClientTypeGeneral = "general";

        /// <summary>
        /// ClientTypeInstitucional.
        /// </summary>
        public const string ClientTypeInstitutional = "institucional";

        /// <summary>
        /// ClientTypeClinica.
        /// </summary>
        public const string ClientTypeClinic = "clinica";

        /// <summary>
        /// Constant to user orders.
        /// </summary>
        public const string UserOrders = "userorder/invoice?type=local";

        /// <summary>
        /// ClientTypeClinica.
        /// </summary>
        public const string GetLine = "getline/invoiceId";

        /// <summary>
        /// Gets the clasification user DZ.
        /// </summary>
        /// <value>
        /// String UserTypeDZ.
        /// </value>
        public static string UserClassificationDZ => "DZ";

        /// <summary>
        /// Gets the close status.
        /// </summary>
        /// <value>
        /// The close status.
        /// </value>
        public static string CloseStatus => "Cerrado";

        /// <summary>
        /// Gets StartDateParam.
        /// </summary>
        /// <value>
        /// String StartDateParam.
        /// </value>
        public static string StartDateParam => "startdate";

        /// <summary>
        /// Gets EndDateParam.
        /// </summary>
        /// <value>
        /// String EndDateParam.
        /// </value>
        public static string EndDateParam => "enddate";

        /// <summary>
        /// Gets GetUserOrdersAlmancen.
        /// </summary>
        /// <value>
        /// String GetUserOrdersAlmancen.
        /// </value>
        public static string EndpointGetUserOrdersAlmancenByRangeDate => "userorders/almacen/byrangedates?startdate={0}&enddate={1}";

        /// <summary>
        /// Gets EndpointGetUserOrdersAlmancenByOrdersId.
        /// </summary>
        /// <value>
        /// String EndpointGetUserOrdersAlmancenByOrdersId.
        /// </value>
        public static string EndpointGetUserOrdersAlmancenByOrdersId => "userorders/almacen/byordersid";

        /// <summary>
        /// Gets EndPointGetLineProductPedidosByRangeDate.
        /// </summary>
        /// <value>
        /// String EndPointGetLineProductPedidosByRangeDate.
        /// </value>
        public static string EndPointGetLineProductPedidosByRangeDate => "almacen/orders/bydaterange";

        /// <summary>
        /// Gets DateTimeFormatddMMyyyy.
        /// </summary>
        /// <value>
        /// String DateTimeFormatddMMyyyy.
        /// </value>
        public static string DateTimeFormatddMMyyyy => "dd/MM/yyyy";

        /// <summary>
        /// Gets GetWareHouseConfigUrl.
        /// </summary>
        /// <value>
        /// String GetWareHouseConfigUrl.
        /// </value>
        public static string GetWareHouseConfigUrl => "warehouse/actives";

        /// <summary>
        /// Gets GetWareHouseConfigUrl.
        /// </summary>
        /// <value>
        /// String GetWareHouseConfigUrl.
        /// </value>
        public static string NoActiveWarehouseError => "El producto no tiene almacenes activos configurados";

        /// <summary>
        /// Gets GetWareHouseConfigUrl.
        /// </summary>
        /// <value>
        /// String GetWareHouseConfigUrl.
        /// </value>
        public static string NoAvaiableBoxesError => "El almacén no cuenta con lotes disponibles";

        /// <summary>
        /// Gets Pedido.
        /// </summary>
        /// <value>
        /// String Pedido.
        /// </value>
        public static string Description => "Pedido {0}";

        /// <summary>
        /// Gets ItemCodeParam.
        /// </summary>
        /// <value>
        /// String ItemCodeParam.
        /// </value>
        public static string ItemCodeParam => "itemcode";

        /// <summary>
        /// Gets WarehouseParam.
        /// </summary>
        /// <value>
        /// String WarehouseParam.
        /// </value>
        public static string WarehouseParam => "warehouse";

        /// <summary>
        /// Gets ConfigRoutesRedisKey.
        /// </summary>
        /// <value>
        /// String ConfigRoutesRedisKey.
        /// </value>
        public static string ConfigRoutesRedisKey => "configroute-valids";

        /// <summary>
        /// Gets GetActiveRouteConfigurationsEndPoint.
        /// </summary>
        /// <value>
        /// String GetActiveRouteConfigurationsEndPoint.
        /// </value>
        public static string GetActiveRouteConfigurationsEndPoint => "active/route/confgurations";

        /// <summary>
        /// Gets MagistralesDbValue.
        /// </summary>
        /// <value>
        /// String MagistralesDbValue.
        /// </value>
        public static string MagistralesDbValue => "MAG";

        /// <summary>
        /// Gets MagistralesDbValue.
        /// </summary>
        /// <value>
        /// String MagistralesDbValue.
        /// </value>
        public static string AlmacenDbValue => "ALM";

        /// <summary>
        /// Gets ProductionOrderSeparationProcessKey.
        /// </summary>
        /// <value>
        /// ProductionOrderSeparationProcessKey.
        /// </value>
        public static string ProductionOrderSeparationProcessKey => "production-order-separation-process:{0}";

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
        public static Dictionary<string, string> DictCatalogGroup { get; } = new Dictionary<string, string>
        {
            { "Magistral Dermocos", MagistralWareHouse },
            { "Magistral Medicament", MagistralWareHouse },
            { "Dermazone", MagistralWareHouse },
            { "Bioequal", "BE" },
            { "Bioelite", "MN" },
            { "MAQUILA", "MP" },
        };

        /// <summary>
        /// Gets the Order Relation.
        /// </summary>
        /// <value>
        /// the OrderRelation.
        /// </value>
        public static Dictionary<string, string> OrderRelation { get; } = new Dictionary<string, string>
        {
            { "Y", "Padre" },
            { "N", "Hija" },
            { "SA", "Completa" },
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

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusToIgnoreUserOrderAdvancedLook { get; } = new List<string>
        {
            Cancelado,
            Abierto,
            Planificado,
            Terminado,
            Almacenado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusToIgnoreLineOrderAdvancedLook { get; } = new List<string>
        {
            Cancelado,
            Almacenado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusForBackOrder { get; } = new List<string>
        {
            Liberado,
            Finalizado,
        };

        /// <summary>
        /// Gets the status of the order liberado.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusForOrderLiberado { get; } = new List<string>
        {
            Almacenado,
            Finalizado,
            Pendiente,
        };

        /// <summary>
        /// Gets list of thw status for the orders.
        /// </summary>
        /// <value>
        /// List of thw status for the orders.
        /// </value>
        public static List<string> ListStatusInvalidDxpDoctor { get; } = new List<string>
        {
            Almacenado,
            Empaquetado,
        };

        /// <summary>
        /// Gets list of thw datasources for the orders.
        /// </summary>
        /// <value>
        /// List of thw status for the orders.
        /// </value>
        public static List<string> DataSources { get; } = new List<string>
        {
            DataSourceDiApi,
            DataSourceServiceLayer,
        };

        /// <summary>
        /// Gets list of client types excluding general.
        /// </summary>
        /// <value>List of client types.</value>
        public static List<string> ClientTypesInstitucionalList { get; } = new List<string>
        {
            ClientTypeInstitutional,
            ClientTypeClinic,
        };

        /// <summary>
        /// Gets the status of the order liberado.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> IsOmigenomicsValue { get; } = new List<string>
        {
            "SI",
            "Y",
            "1",
        };

        /// <summary>
        /// Gets the Default Filters.
        /// </summary>
        /// <value>
        /// default filters.
        /// </value>
        public static List<string> DefaultFilters { get; } =
        [
            "mixto",
            "maquila",
            "muestra",
            "paquetes",
            "omigenomics",
        ];

        /// <summary>
        /// Gets the Default Filters.
        /// </summary>
        /// <value>
        /// default filters.
        /// </value>
        public static List<string> InvalidClassifications { get; } =
        [
            "MQ",
            "MX",
        ];

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        /// <value>
        /// descriptions.
        /// </value>
        public static Dictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
        {
            { "De Linea", "De Línea" },
        };

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        /// <value>
        /// descriptions.
        /// </value>
        public static Dictionary<string, string> Filter { get; } = new Dictionary<string, string>
        {
            { ServiceConstants.Maquila.ToUpper(), "MQ" },
        };

        /// <summary>
        /// Gets the Default Filters.
        /// </summary>
        /// <value>
        /// default filters.
        /// </value>
        public static List<string> InvalidCatalogsGroups { get; } =
        [
            "Envases",
            "Paqueteria",
        ];
    }
}
