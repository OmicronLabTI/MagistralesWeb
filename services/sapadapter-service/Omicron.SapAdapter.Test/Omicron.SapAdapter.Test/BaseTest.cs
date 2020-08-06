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
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina" },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 100, ProductoId = "Buscapina" },
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
                new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina" },
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
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100 },
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
                new UserOrderModel { Id = 1, Productionorderid = "12", Salesorderid = "12", Status = "Abierto", Userid = "123" },
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
