// <summary>
// <copyright file="BaseTestAdvancedLookUp.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public class BaseTestAdvancedLookUp
    {
        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultGetUserPedidos()
        {
            var listUsers = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1039, Productionorderid = null, Salesorderid = "84434", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2021, 03, 23), DateTimeCheckIn = new DateTime(2021, 03, 24), StatusAlmacen = "Back Order", DeliveryId = 0, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 1131, Productionorderid = "122771", Salesorderid = "84434", Status = "Almacenado", Userid = "123", CloseDate = new DateTime(2021, 03, 23), DateTimeCheckIn = new DateTime(2021, 03, 24), StatusAlmacen = "Almacenado", DeliveryId = 74709, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 345, Productionorderid = "122260", Salesorderid = "84132", Status = "Entregado", Userid = "123", CloseDate = new DateTime(2021, 05, 02), DateTimeCheckIn = new DateTime(2021, 05, 02), StatusAlmacen = "Empaquetado", DeliveryId = 74463, StatusInvoice = "Empaquetado", InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 1131, Productionorderid = "122771", Salesorderid = "84434", Status = "Almacenado", Userid = "123", CloseDate = new DateTime(2021, 03, 23), DateTimeCheckIn = new DateTime(2021, 03, 24), StatusAlmacen = "Almacenado", DeliveryId = 74709, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },

                new UserOrderModel { Id = 1135, Productionorderid = "122790", Salesorderid = "84458", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2021, 04, 05), DateTimeCheckIn = null, StatusAlmacen = null, DeliveryId = 0, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 1136, Productionorderid = null, Salesorderid = "84458", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2021, 04, 05), DateTimeCheckIn = new DateTime(2021, 04, 05), StatusAlmacen = "Back Order", DeliveryId = 0, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 1156, Productionorderid = "122789", Salesorderid = "84458", Status = "Almacenado", Userid = "123", CloseDate = new DateTime(2021, 04, 05), DateTimeCheckIn = new DateTime(2021, 04, 05), StatusAlmacen = "Almacenado", DeliveryId = 74728, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },

                new UserOrderModel { Id = 1165, Productionorderid = "122798", Salesorderid = "84473", Status = "Almacenado", Userid = "123", CloseDate = new DateTime(2021, 04, 06), DateTimeCheckIn = new DateTime(2021, 04, 06), StatusAlmacen = "Empaquetado", DeliveryId = 74751, StatusInvoice = "Empaquetado", InvoiceStoreDate = new DateTime(2021, 04, 06), InvoiceId = 115024 },
                new UserOrderModel { Id = 1166, Productionorderid = null, Salesorderid = "84473", Status = "Almacendo", Userid = "123", CloseDate = new DateTime(2021, 04, 06), DateTimeCheckIn = new DateTime(2021, 04, 06), StatusAlmacen = "Almacenado", DeliveryId = 74751, StatusInvoice = "Empaquetado", InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 553, Productionorderid = "122275", Salesorderid = "84144", Status = "Entregado", Userid = "123", CloseDate = new DateTime(2021, 02, 08), DateTimeCheckIn = new DateTime(2021, 02, 08), StatusAlmacen = "Almacenado", DeliveryId = 74473, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },
                new UserOrderModel { Id = 561, Productionorderid = "122275", Salesorderid = "84144", Status = "Entregado", Userid = "123", CloseDate = new DateTime(2021, 02, 08), DateTimeCheckIn = new DateTime(2021, 02, 08), StatusAlmacen = "Almacenado", DeliveryId = 74473, StatusInvoice = null, InvoiceStoreDate = null, InvoiceId = 0 },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(listUsers),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultGetAdvancedModelAlmacen()
        {
            var lineProducts = new List<LineProductsModel>()
            {
                new LineProductsModel { Id = 4139, SaleOrderId = 84434, StatusAlmacen = "Empaquetado", StatusInvoice = null, ItemCode = "REVE 14", InvoiceStoreDate = new DateTime(2021, 03, 30), InvoiceId = 115010, DeliveryId = 74709, DateCheckIn = new DateTime(2021, 03, 24) },
                new LineProductsModel { Id = 4140, SaleOrderId = 84434, StatusAlmacen = "Almacenado", StatusInvoice = null, ItemCode = "REVE 22", InvoiceStoreDate = null, InvoiceId = 0, DeliveryId = 74710, DateCheckIn = new DateTime(2021, 03, 24) },

                new LineProductsModel { Id = 4139, SaleOrderId = 84434, StatusAlmacen = "Empaquetado", StatusInvoice = null, ItemCode = "REVE 14", InvoiceStoreDate = new DateTime(2021, 03, 30), InvoiceId = 115010, DeliveryId = 74709, DateCheckIn = new DateTime(2021, 03, 24) },
                new LineProductsModel { Id = 4140, SaleOrderId = 84434, StatusAlmacen = "Almacenado", StatusInvoice = null, ItemCode = "REVE 22", InvoiceStoreDate = null, InvoiceId = 0, DeliveryId = 74710, DateCheckIn = new DateTime(2021, 03, 24) },
                new LineProductsModel { Id = 4139, SaleOrderId = 84434, StatusAlmacen = "Empaquetado", StatusInvoice = null, ItemCode = "REVE 14", InvoiceStoreDate = new DateTime(2021, 03, 30), InvoiceId = 115010, DeliveryId = 74709, DateCheckIn = new DateTime(2021, 03, 24) },

                new LineProductsModel { Id = 4171, SaleOrderId = 84474, StatusAlmacen = "Almacenado", StatusInvoice = null, ItemCode = "REVE 14", InvoiceStoreDate = null, InvoiceId = 0, DeliveryId = 74752, DateCheckIn = new DateTime(2021, 04, 06) },
                new LineProductsModel { Id = 4172, SaleOrderId = 84474, StatusAlmacen = "Almacenado", StatusInvoice = null, ItemCode = null, InvoiceStoreDate = null, InvoiceId = 0, DeliveryId = 74752, DateCheckIn = new DateTime(2021, 04, 06) },
            };

            var packageModels = new List<PackageModel>()
            {
                new PackageModel { AssignedDate = new DateTime(2021, 12, 02), AssignedUser = "1", Comments = "Usuario 1", DeliveredDate = new DateTime(2021, 12, 02), InWayDate = new DateTime(2021, 12, 02), InvoiceId = 115024, Status = "Entregado" },
            };

            var cancelationModel = new List<CancellationResourceModel>()
            {
                new CancellationResourceModel { Id = 4, CancelDate = new DateTime(2021, 04, 06), CancelledId = 115025, TypeCancellation = "invoice" },
                new CancellationResourceModel { Id = 7, CancelDate = new DateTime(2021, 04, 08), CancelledId = 74746, TypeCancellation = "delivery" },
            };

            var adnvaceLookUpModel = new AdnvaceLookUpModel
            {
                LineProducts = lineProducts,
                PackageModels = packageModels,
                CancelationModel = cancelationModel,
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(adnvaceLookUpModel),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<OrderModel> GetOrderModel()
        {
            return new List<OrderModel>
            {
                new OrderModel { PedidoId = 84434,  DocNum = 84434, FechaInicio = new DateTime(2021, 03, 23), Medico = "Medico B", Codigo = "MB", PedidoStatus = "O", Address = "Monterrey,Nuevo León", OrderType = "MX", AsesorId = 125 },
                new OrderModel { PedidoId = 85000, DocNum = 85000, FechaInicio = DateTime.Today.AddDays(-30), Medico = "Medico B", Codigo = "MB", PedidoStatus = "O", Address = "CDMX", OrderType = "MQ", AsesorId = 125 },

                new OrderModel { PedidoId = 84458, DocNum = 84458, FechaInicio = new DateTime(2021, 04, 01), Medico = "Medico A", Codigo = "MA", PedidoStatus = "O", Address = "Guadalajara", OrderType = "BE", AsesorId = 125 },

                new OrderModel { PedidoId = 84473, DocNum = 84473, FechaInicio = new DateTime(2021, 03, 06), Medico = "Medico A", Codigo = "MA", PedidoStatus = "C", Address = "Puebla", OrderType = "BE", AsesorId = 125 },
                new OrderModel { PedidoId = 84508, DocNum = 84508, FechaInicio = new DateTime(2021, 04, 08), Medico = "Medico A", Codigo = "MA", PedidoStatus = "O", Address = "Monterrey,Nuevo León", OrderType = "MX", AsesorId = 125 },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public AsesorModel GetAsesorModel()
        {
            return new AsesorModel
            {
                AsesorId = 125,
                AsesorName = "Gustavo",
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetUsers()
        {
            var listUsers = new List<UserModel>
            {
                new UserModel { Id = "1", Activo = 1, FirstName = "juanito", Asignable = 2, Piezas = 5, Role = 5 },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(listUsers),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<DetallePedidoModel> GetDetallePedido()
        {
            return new List<DetallePedidoModel>
            {
                new DetallePedidoModel { Description = "descripcion A", DetalleId = 0, PedidoId = 84434, ProductoId = "104   120 ML", Container = "TARRO", Quantity = 20, DestinyAddress = "MONTERREY , Nuevo León,", DocDate = new DateTime(2021, 03, 23) },
                new DetallePedidoModel { Description = "descripcion B", DetalleId = 1, PedidoId = 84434, ProductoId = "REVE 14", Container = "TARRO", Quantity = 10, DestinyAddress = "MONTERREY , Nuevo León,", DocDate = new DateTime(2021, 03, 23) },
                new DetallePedidoModel { Description = "descripcion c", DetalleId = 2, PedidoId = 84434, ProductoId = "REVE 21", Container = "AMBAR", Quantity = 15, DestinyAddress = "MONTERREY , Nuevo León,", DocDate = new DateTime(2021, 03, 23) },
                new DetallePedidoModel { Description = "descripcion D", DetalleId = 3, PedidoId = 84434, ProductoId = "REVE 22", Container = "ESPECIAL", Quantity = 5, DestinyAddress = "MONTERREY , Nuevo León,", DocDate = new DateTime(2021, 03, 23) },

                new DetallePedidoModel { Description = "descripcion E", DetalleId = 0, PedidoId = 84458, ProductoId = "567   120 ML", Container = "AMBAR", Quantity = 1, DestinyAddress = "Guadalajara", DocDate = new DateTime(2021, 04, 05) },
                new DetallePedidoModel { Description = "descripcion F", DetalleId = 1, PedidoId = 84458, ProductoId = "567   60 MLML", Container = "BQ", Quantity = 1, DestinyAddress = "Guadalajara", DocDate = new DateTime(2021, 04, 05) },

                new DetallePedidoModel { Description = "descripcion E", DetalleId = 0, PedidoId = 84473, ProductoId = "567   60 ML", Container = "AMBAR", Quantity = 1, DestinyAddress = "Puebla", DocDate = new DateTime(2021, 03, 06) },
                new DetallePedidoModel { Description = "descripcion E", DetalleId = 0, PedidoId = 84508, ProductoId = "REVE 14", Container = "AMBAR", Quantity = 1, DestinyAddress = "MONTERREY , Nuevo León,", DocDate = new DateTime(2021, 04, 08) },
            };
        }

        /// <summary>
        /// Gets the deliveries.
        /// </summary>
        /// <returns>the data.</returns>
        public List<DeliveryDetailModel> GetDeliveryDetail()
        {
            return new List<DeliveryDetailModel>
            {
                new DeliveryDetailModel { BaseEntry = 84434, DeliveryId = 74709, Description = "Dsc", DocDate = new DateTime(2021, 03, 23), ProductoId = "104   120 ML", Quantity = 20, InvoiceId = 38507 },
                new DeliveryDetailModel { BaseEntry = 84434, DeliveryId = 74709, Description = "Dsc", DocDate = new DateTime(2021, 03, 23), ProductoId = "REVE 14", Quantity = 10, InvoiceId = 38507, LineNum = 2 },
                new DeliveryDetailModel { BaseEntry = 84434, DeliveryId = 74710, Description = "Dsc", DocDate = new DateTime(2021, 03, 23), ProductoId = "REVE 22", Quantity = 5, InvoiceId = 38506 },

                new DeliveryDetailModel { BaseEntry = 84132, DeliveryId = 74463, Description = "Dsc", DocDate = new DateTime(2021, 05, 02), ProductoId = "104   120 ML", Quantity = 2, InvoiceId = 38453 },
                new DeliveryDetailModel { BaseEntry = 84458, DeliveryId = 74728, Description = "Dsc", DocDate = new DateTime(2021, 04, 05), ProductoId = "567   60 ML", Quantity = 1, InvoiceId = null },

                new DeliveryDetailModel { BaseEntry = 84473, DeliveryId = 74751, Description = "Dsc", DocDate = new DateTime(2021, 03, 06), ProductoId = "567   60 ML", Quantity = 1, InvoiceId = 38521 },
                new DeliveryDetailModel { BaseEntry = 84474, DeliveryId = 74752, Description = "Dsc", DocDate = new DateTime(2021, 04, 06), ProductoId = "REVE 14", Quantity = 1, InvoiceId = 38524, LineStatus = "C" },
                new DeliveryDetailModel { BaseEntry = 84144, DeliveryId = 74473, Description = "Dsc", DocDate = new DateTime(2021, 02, 08), ProductoId = "567   120 ML", Quantity = 3, InvoiceId = 38463, LineStatus = "C", LineNum = 1 },
                new DeliveryDetailModel { BaseEntry = 84144, DeliveryId = 74473, Description = "Dsc", DocDate = new DateTime(2021, 02, 08), ProductoId = "149   60 ML", Quantity = 4, InvoiceId = 38463, LineStatus = "C" },

                new DeliveryDetailModel { BaseEntry = 84469, DeliveryId = 74746, Description = "Dsc", DocDate = new DateTime(2021, 04, 06), ProductoId = "567   30 ML", Quantity = 1, InvoiceId = null },
            };
        }

        /// <summary>
        /// Gets the delivery header.
        /// </summary>
        /// <returns>the data.</returns>
        public List<DeliverModel> DeliveryModel()
        {
            return new List<DeliverModel>
            {
                new DeliverModel { Cliente = "Cliente A", DeliveryStatus = "C", DocNum = 74709, FechaInicio = new DateTime(2021, 03, 24), Medico = "Medico A", CardCode = "MA", PedidoId = 74709, Address = "MONTERREY,Nuevo León" },
                new DeliverModel { Cliente = "Cliente B", DeliveryStatus = "C", DocNum = 74710, FechaInicio = new DateTime(2021, 03, 24), Medico = "Medico A", CardCode = "MA", PedidoId = 74710, Address = "MONTERREY ,Nuevo León" },
                new DeliverModel { Cliente = "Cliente C", DeliveryStatus = "O", DocNum = 74728, FechaInicio = new DateTime(2021, 04, 05), Medico = "Medico B", CardCode = "MB", PedidoId = 74728, Address = "Guadalajara" },

                new DeliverModel { Cliente = "Cliente B", DeliveryStatus = "C", DocNum = 74751, FechaInicio = new DateTime(2021, 04, 06), Medico = "Medico B", CardCode = "MB", PedidoId = 74751, Address = "Puebla" },

                new DeliverModel { Cliente = "Cliente F", DeliveryStatus = "O", DocNum = 74746, FechaInicio = new DateTime(2021, 04, 06), Medico = "Medico B", CardCode = "MB", PedidoId = 74746, Address = "Guadalajara" },
            };
        }

        /// <summary>
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<InvoiceHeaderModel> GetInvoiceHeader()
        {
            return new List<InvoiceHeaderModel>
            {
                new InvoiceHeaderModel { Address = "Queretaro, Mexico,", Cliente = "cliente A", CardCode = "C1", DocNum = 111827, FechaInicio = new DateTime(2020, 10, 29), InvoiceId = 35147, InvoiceStatus = "C", Medico = "Medico A", SalesPrsonId = 40, Canceled = "N" },
                new InvoiceHeaderModel { Address = "MONTERREY ,\rNuevo León, Mexico", Cliente = "cliente B", CardCode = "C8", DocNum = 115009, FechaInicio = new DateTime(2021, 03, 24), InvoiceId = 38506, InvoiceStatus = "O", Medico = "Medico B", SalesPrsonId = 16, Canceled = "N" },
                new InvoiceHeaderModel { Address = "MONTERREY ,\rNuevo León, Mexico", Cliente = "cliente C", CardCode = "C1", DocNum = 115010, FechaInicio = new DateTime(2021, 03, 24), InvoiceId = 38507, InvoiceStatus = "O", Medico = "Medico B", SalesPrsonId = 16, Canceled = "N" },

                new InvoiceHeaderModel { Address = "Puebla", Cliente = "cliente A", CardCode = "C1", DocNum = 115024, FechaInicio = new DateTime(2021, 04, 06), InvoiceId = 38521, InvoiceStatus = "O", Medico = "Medico A", SalesPrsonId = 12, Canceled = "N" },
                new InvoiceHeaderModel { Address = "TECAMACHALCO Puebla", Cliente = "cliente F", CardCode = "C03911", DocNum = 115025, FechaInicio = new DateTime(2021, 04, 06), InvoiceId = 38522, InvoiceStatus = "C", Medico = "Medico A", Canceled = "Y" },
                new InvoiceHeaderModel { Address = "MONTERREY ,\rNuevo León, Mexico", Cliente = "cliente S", CardCode = "C00005", DocNum = 114966, FechaInicio = new DateTime(2021, 02, 10), InvoiceId = 38463, InvoiceStatus = "O", Medico = "Medico A", Canceled = "N" },

                new InvoiceHeaderModel { Address = "Jalisco,  Mexico", Cliente = "cliente S", CardCode = "C00214", DocNum = 84508, FechaInicio = new DateTime(2019, 05, 15), InvoiceId = 6819, InvoiceStatus = "C", Medico = "Medico B", Canceled = "N" },
            };
        }

        /// <summary>
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<InvoiceDetailModel> GetInvoiceDetails()
        {
            return new List<InvoiceDetailModel>
            {
                new InvoiceDetailModel { BaseEntry = 74709, Container = "con", Description = "desc", DocDate = new DateTime(2021, 03, 24), InvoiceId = 38507, LineNum = 0, ProductoId = "104   120 ML", Quantity = 20 },
                new InvoiceDetailModel { BaseEntry = 74709, Container = "con", Description = "desc", DocDate = new DateTime(2021, 03, 24), InvoiceId = 38507, LineNum = 1, ProductoId = "REVE 14", Quantity = 10 },
                new InvoiceDetailModel { BaseEntry = 74709, Container = "con", Description = "desc", DocDate = new DateTime(2020, 10, 28), InvoiceId = 35147, LineNum = 4, ProductoId = "DZ 28", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = 74710, Container = "con", Description = "desc", DocDate = new DateTime(2021, 03, 24), InvoiceId = 38506, LineNum = 0, ProductoId = "REVE 22", Quantity = 5 },

                new InvoiceDetailModel { BaseEntry = 74463, Container = "con", Description = "desc", DocDate = new DateTime(2020, 10, 27), InvoiceId = 35147, LineNum = 5, ProductoId = "2777   30 ML", Quantity = 5 },
                new InvoiceDetailModel { BaseEntry = 74463, Container = "con", Description = "desc", DocDate = new DateTime(2020, 10, 27), InvoiceId = 35147, LineNum = 9, ProductoId = "DZ 28", Quantity = 3 },
                new InvoiceDetailModel { BaseEntry = 74463, Container = "con", Description = "desc", DocDate = new DateTime(2020, 10, 27), InvoiceId = 35147, LineNum = 7, ProductoId = "REVE 32", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = 74463, Container = "con", Description = "desc", DocDate = new DateTime(2020, 10, 27), InvoiceId = 35147, LineNum = 8, ProductoId = "DZ 49", Quantity = 1 },

                new InvoiceDetailModel { BaseEntry = 74709, Container = "con", Description = "desc", DocDate = new DateTime(2020, 10, 28), InvoiceId = 35147, LineNum = 6, ProductoId = "DZ 28", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = 74710, Container = "con", Description = "desc", DocDate = new DateTime(2021, 03, 24), InvoiceId = 38506, LineNum = 10, ProductoId = "REVE 22", Quantity = 5 },
                new InvoiceDetailModel { BaseEntry = 74709, Container = "con", Description = "desc", DocDate = new DateTime(2021, 03, 24), InvoiceId = 35147, LineNum = 11, ProductoId = "104   120 ML", Quantity = 20 },
                new InvoiceDetailModel { BaseEntry = 74709, Container = "con", Description = "desc", DocDate = new DateTime(2021, 03, 24), InvoiceId = 38507, LineNum = 12, ProductoId = "REVE 14", Quantity = 10 },

                new InvoiceDetailModel { BaseEntry = 74751, Container = "con", Description = "desc", DocDate = new DateTime(2021, 04, 06), InvoiceId = 38521, LineNum = 0, ProductoId = "567   60 ML", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = 74752, Container = "con", Description = "desc", DocDate = new DateTime(2021, 04, 06), InvoiceId = 38522, LineNum = 0, ProductoId = "REVE 14", Quantity = 1 },

                new InvoiceDetailModel { BaseEntry = 74473, Container = "con", Description = "desc", DocDate = new DateTime(2021, 02, 08), InvoiceId = 38463, LineNum = 0, ProductoId = "567   120 ML", Quantity = 3 },
                new InvoiceDetailModel { BaseEntry = 74473, Container = "con", Description = "desc", DocDate = new DateTime(2021, 02, 08), InvoiceId = 38463, LineNum = 1, ProductoId = "149   60 ML", Quantity = 4 },

                new InvoiceDetailModel { BaseEntry = null, Container = "con", Description = "desc", DocDate = new DateTime(2019, 04, 12), InvoiceId = 6819, LineNum = 0, ProductoId = "DZ 45", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = null, Container = "con", Description = "desc", DocDate = new DateTime(2019, 04, 12), InvoiceId = 6819, LineNum = 1, ProductoId = "DZ 49", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = null, Container = "con", Description = "desc", DocDate = new DateTime(2019, 04, 12), InvoiceId = 6819, LineNum = 2, ProductoId = "DZ 38", Quantity = 6 },
            };
        }

        /// <summary>
        /// get the product.
        /// </summary>
        /// <returns>the product.</returns>
        public List<ProductoModel> GetProductoModel()
        {
            return new List<ProductoModel>
            {
                new ProductoModel { IsMagistral = "N", ProductoId = "REVE 14", ProductoName = "Ungüento 10 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Ungüento 10 GR", IsLine = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "REVE 21", ProductoName = "Cápsula 12 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Cápsula 12 GR", IsLine = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "REVE 22", ProductoName = "Ungüento 10 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Ungüento 10 GR", IsLine = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "DZ 28", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "REVE 32", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "DZ 49", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y" },

                new ProductoModel { IsMagistral = "N", ProductoId = "DZ 45", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "DZ 38", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y" },

                new ProductoModel { IsMagistral = "Y", ProductoId = "104   120 ML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "567   120 ML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "567   60 MLML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "567   60 ML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "149   60 ML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "567   30 ML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "2777   30 ML", ProductoName = "product name", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "N" },
            };
        }

        /// <summary>
        /// get the product.
        /// </summary>
        /// <returns>the product.</returns>
        public List<Repartidores> GetRepartidores()
        {
            return new List<Repartidores>
            {
               new Repartidores { TrnspCode = 1, TrnspName = "DHL" },
            };
        }

        /// <summary>
        /// get the doctors.
        /// </summary>
        /// <returns>the product.</returns>
        public List<ClientCatalogModel> GetDoctorsModels()
        {
            return new List<ClientCatalogModel>
            {
                new ClientCatalogModel { ClientId = "MB", AliasName = "médico B alias", Email = "email@email" },
                new ClientCatalogModel { ClientId = "MA", AliasName = "médico A alias", Email = "email@email" },
                new ClientCatalogModel { ClientId = "C1", AliasName = "médico C1 alias", Email = "email@email" },
                new ClientCatalogModel { ClientId = "C8", AliasName = "médico C8 alias", Email = "email@email" },
                new ClientCatalogModel { ClientId = "C03911", AliasName = "médico C03911 alias", Email = "email@email" },
                new ClientCatalogModel { ClientId = "C00005", AliasName = "médico C00005 alias", Email = "email@email" },
                new ClientCatalogModel { ClientId = "C00214", AliasName = "médico C00214 alias", Email = "email@email" },
            };
        }
    }
}
