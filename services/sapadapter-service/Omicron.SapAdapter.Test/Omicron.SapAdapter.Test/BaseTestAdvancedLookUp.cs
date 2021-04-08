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
            };

            var packageModels = new List<PackageModel>();

            var cancelationModel = new List<CancellationResourceModel>();

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
                new OrderModel { PedidoId = 84434, Cliente = "Cliente A", DocNum = 84434, FechaInicio = new DateTime(2021, 03, 23), Medico = "Medico A", PedidoStatus = "O", Address = "Monterrey,Nuevo León", OrderType = "MX" },
                new OrderModel { PedidoId = 85000, Cliente = "cliente", DocNum = 85000, FechaInicio = DateTime.Today.AddDays(-30), Medico = "Medico", PedidoStatus = "O", Address = "CDMX", OrderType = "MQ" },
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

                new DeliveryDetailModel { BaseEntry = 84132, DeliveryId = 74463, Description = "Dsc", DocDate = new DateTime(2021, 05, 02), ProductoId = "567   120 ML", Quantity = 2, InvoiceId = 38453 },
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
                new DeliverModel { Cliente = "Cliente A", DeliveryStatus = "C", DocNum = 74709, FechaInicio = new DateTime(2021, 03, 24), Medico = "Medico A", PedidoId = 74709, Address = "MONTERREY,Nuevo León" },
                new DeliverModel { Cliente = "Cliente B", DeliveryStatus = "C", DocNum = 74710, FechaInicio = new DateTime(2021, 03, 24), Medico = "Medico B", PedidoId = 74710, Address = "MONTERREY ,Nuevo León" },
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
                new InvoiceHeaderModel { Address = "MONTERREY ,\rNuevo León, Mexico", Cliente = "cliente C", CardCode = "C1", DocNum = 115010, FechaInicio = new DateTime(2021, 03, 24), InvoiceId = 38507, InvoiceStatus = "O", Medico = "Medico C", SalesPrsonId = 16, Canceled = "N" },
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
    }
}
