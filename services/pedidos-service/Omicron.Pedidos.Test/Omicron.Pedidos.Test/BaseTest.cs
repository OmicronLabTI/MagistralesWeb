// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Dtos.User;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;

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
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public UserOrderModel GetUserOrderModel()
        {
            return new UserOrderModel
            {
                Id = 1,
                Productionorderid = "123",
                Salesorderid = "122",
                Status = "Planificado",
                Userid = "111",
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public OrderLogModel GetOrderLogModel()
        {
            return new OrderLogModel
            {
                Userid = "111",
                Id = 1,
                Description = "description",
                Logdatetime = DateTime.Now,
                Noid = "112",
                Type = "OF",
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelGetFabricacionModel()
        {
            var listOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { DataSource = "O", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProductoId = "Aspirina", Quantity = 10, Status = "R" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultCreateOrder()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-Aspirina", ServiceConstants.Ok },
                { "200-Aspirina", ServiceConstants.ErrorCreateFabOrd },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }
    }
}
