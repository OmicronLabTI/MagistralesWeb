// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;

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
                new UserModel { Id = "1", FirstName = "Alejandro", LastName = "Ojeda", UserName = "Alex", Password = "QXhpdHkyMDIw", Role = 1, Activo = 1, Piezas = 200, Asignable = 1, Deleted = true, },
                new UserModel { Id = "2", FirstName = "Jorge", LastName = "Morales", UserName = "George", Password = "QXhpdHkyMDIw", Role = 2, Activo = 1, Piezas = 200, Asignable = 1, Deleted = true, },
                new UserModel { Id = "3", FirstName = "Arturo", LastName = "Miranda", UserName = "Artuhr", Password = "QXhpdHkyMDIw", Role = 2, Activo = 1, Piezas = 200, Asignable = 1, Deleted = true, },
                new UserModel { Id = "4", FirstName = "Benjamin", LastName = "Galindo", UserName = "Benji", Password = "QXhpdHkyMDIw", Role = 2, Activo = 1, Piezas = 200, Asignable = 1, Deleted = true, },
                new UserModel { Id = "5", FirstName = "Benjamin", LastName = "Galindo", UserName = "Benji", Password = "QXhpdHkyMDIw", Role = 1, Activo = 1, Piezas = 200, Asignable = 1, Deleted = true, },
                new UserModel { Id = "6", FirstName = "Benjamin", LastName = "Galindo", UserName = "Benji", Password = "QXhpdHkyMDIw", Role = 1, Activo = 1, Piezas = 200, Asignable = 1, Deleted = true, },
                new UserModel { Id = "7", FirstName = "Usuario7", LastName = "Usuario7", UserName = "Usuario7", Password = "QXhpdHkyMDIw", Role = 1, Activo = 0, Piezas = 200, Asignable = 1, Deleted = true, },
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
                Id = "10",
                FirstName = "Jorge",
                LastName = "Morales",
                UserName = "George",
                Password = "abc",
                Role = 1,
                Activo = 1,
                Asignable = 1,
                Piezas = 200,
            };
        }

        /// <summary>
        /// Returns a user model.
        /// </summary>
        /// <returns>the return model.</returns>
        public UserModel GetUserModel()
        {
            return new UserModel
            {
                Id = "10",
                FirstName = "Jorge",
                LastName = "Morales",
                UserName = "George",
                Password = "abc",
                Role = 1,
                Activo = 1,
                Piezas = 200,
                Asignable = 1,
            };
        }

        /// <summary>
        /// gets the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        public List<RoleModel> GetRoles()
        {
            return new List<RoleModel>
            {
                new RoleModel { Description = "qfb", Id = 2 },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultCreateOrder()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 2, Productionorderid = "100", Salesorderid = "100", Status = "Asignado", Userid = "2" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(userOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultFabOrders()
        {
            var userOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { CreatedDate = DateTime.Now, DataSource = "M", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProdName = "name", ProductoId = "Aspirina", Quantity = 10, Status = "status" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(userOrders),
                Success = true,
                UserError = string.Empty,
            };
        }
    }
}
