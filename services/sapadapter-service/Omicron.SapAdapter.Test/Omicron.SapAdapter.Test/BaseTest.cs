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
    using Omicron.SapAdapter.Dtos.User;
    using Omicron.SapAdapter.Entities.Model;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// List of Users.
        /// </summary>
        /// <returns>IEnumerable Users.</returns>
        public IEnumerable<UserModel> GetAllUsers()
        {
            return new List<UserModel>()
            {
                new UserModel { Id = 1, FirstName = "Alejandro", LastName = "Ojeda", Email = "alejandro.ojeda@axity.com", Birthdate = DateTime.Now },
                new UserModel { Id = 2, FirstName = "Jorge", LastName = "Morales", Email = "jorge.morales@axity.com", Birthdate = DateTime.Now },
                new UserModel { Id = 3, FirstName = "Arturo", LastName = "Miranda", Email = "arturo.miranda@axity.com", Birthdate = DateTime.Now },
                new UserModel { Id = 4, FirstName = "Benjamin", LastName = "Galindo", Email = "benjamin.galindo@axity.com", Birthdate = DateTime.Now },
            };
        }

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
    }
}
