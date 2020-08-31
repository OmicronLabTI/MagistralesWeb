// <summary>
// <copyright file="BaseTest.cs" company="Axity">
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
    using Omicron.SapAdapter.Dtos.User;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public UserDto GetUserDto()
        {
            return new UserDto
            {
                Id = 10,
                FirstName = "Jorge",
                LastName = "Morales",
                Email = "test@test.com",
                Birthdate = DateTime.Now,
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
                AsesorId = 1,
                AsesorName = "Gustavo",
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
                new OrderModel { PedidoId = 100, AsesorId = 1, Cliente = "cliente", Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C" },
                new OrderModel { PedidoId = 101, AsesorId = 1, Cliente = "cliente", Codigo = "Codigo", DocNum = 101, FechaFin = DateTime.Today.AddDays(1), FechaInicio = DateTime.Today, Medico = "Medico", PedidoStatus = "O" },
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
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina", Container = "NA", Label = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 100, ProductoId = "Buscapina", Container = "NA", Label = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 101, ProductoId = "Abc Aspirina", Container = "NA", Label = "NA", Quantity = 10 },
            };
        }

        /// <summary>
        /// returns the user.
        /// </summary>
        /// <returns>the user.</returns>
        public List<Users> GetSapUsers()
        {
            return new List<Users>
            {
                new Users { UserId = 1, UserName = "Gus" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<DetalleFormulaModel> GetDetalleFormula()
        {
            return new List<DetalleFormulaModel>
            {
                new DetalleFormulaModel { Almacen = "MN", BaseQuantity = 10, ConsumidoQty = 10, ItemCode = "Abc Aspirina", LineNum = 1, OrderFabId = 100, RequiredQty = 100, UnidadCode = "KG" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<ItemWarehouseModel> GetItemWareHouse()
        {
            return new List<ItemWarehouseModel>
            {
                new ItemWarehouseModel { IsCommited = 10, ItemCode = "Abc Aspirina", OnHand = 10, OnOrder = 10, WhsCode = "MN" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<Batches> GetBatches()
        {
            return new List<Batches>
            {
                new Batches { AbsEntry = 1, DistNumber = "Lote1", ItemCode = "Abc Aspirina", SysNumber = 1 },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<BatchesQuantity> GetBatchesQuantity()
        {
            return new List<BatchesQuantity>
            {
                new BatchesQuantity { AbsEntry = 1, ItemCode = "Abc Aspirina", SysNumber = 1, CommitQty = 10, Quantity = 10, WhsCode = "MN" },
            };
        }

        /// <summary>
        /// returns  data.
        /// </summary>
        /// <returns>the data.</returns>
        public List<BatchTransacitions> GetBatchTransacitions()
        {
            return new List<BatchTransacitions>
            {
                new BatchTransacitions { ItemCode = "Abc Aspirina", LogEntry = 1, DocNum = 100, DocQuantity = 10 },
            };
        }

        /// <summary>
        /// gets the transaction qty model.
        /// </summary>
        /// <returns>the model.</returns>
        public List<BatchesTransactionQtyModel> GetBatchesTransactionQtyModel()
        {
            return new List<BatchesTransactionQtyModel>
            {
                new BatchesTransactionQtyModel { AllocQty = 1, LogEntry = 1, ItemCode = "Abc Aspirina", SysNumber = 1 },
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
                new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Aspirina con 2%" },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<OrdenFabricacionModel> GetOrdenFabricacionModel()
        {
            return new List<OrdenFabricacionModel>
            {
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 100, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 110, PostDate = DateTime.Now, Quantity = 1, Status = "L", PedidoId = 0, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 0, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 120, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 100, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 130, PostDate = DateTime.Now, Quantity = 1, Status = "L", PedidoId = 0, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 0, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<CompleteDetailOrderModel> GetCompleteDetailOrderModel()
        {
            return new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "Abc Aspirina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", },
                new CompleteDetailOrderModel { CodigoProducto = "Buscapina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 101, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", },
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultGetUserPedidos()
        {
            var listUsers = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Productionorderid = "12", Salesorderid = "12", Status = "Abierto", Userid = "123", CloseDate = "20/01/2020", Comments = "comments", FinishDate = "20/01/2020" },
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
        public ResultDto GetResultDtoGetUsersById()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "123", Activo = 1, FirstName = "Gus", LastName = "Ramirez", Password = "asd", Role = 1, UserName = "asdf" },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(users),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }
    }
}
