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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Dtos.User;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;

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
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserOrderModel()
        {
            return new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Productionorderid = "100", Salesorderid = "100", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020", CloseDate = "28/08/2020", CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc" },
                new UserOrderModel { Id = 2, Productionorderid = "101", Salesorderid = "100", Status = "Proceso", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020", CloseDate = "28/08/2020", CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc" },
                new UserOrderModel { Id = 3, Productionorderid = "102", Salesorderid = "100", Status = "Terminado", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020", CloseDate = "28/08/2020", CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc" },
                new UserOrderModel { Id = 4, Productionorderid = "103", Salesorderid = "100", Status = "Reasignado", Userid = "abc", Comments = null, FinishDate = "29/08/2020", CloseDate = "28/08/2020", CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc" },
                new UserOrderModel { Id = 5, Productionorderid = null, Salesorderid = "100", Status = "Terminado", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020", CloseDate = "28/08/2020", CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc" },
                new UserOrderModel { Id = 6, Productionorderid = null, Salesorderid = "100", Status = "Reasignado", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020", CloseDate = "28/08/2020", CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc" },

                // Cancelled orders.
                new UserOrderModel { Id = 7, Productionorderid = null, Salesorderid = "100", Status = "Terminado", Userid = "abcd", Comments = "Hello", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 8, Productionorderid = null, Salesorderid = "100", Status = "Reasignado", Userid = "abcd", Comments = "Hello", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 9, Productionorderid = null, Salesorderid = "101", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 10, Productionorderid = "104", Salesorderid = "103", Status = "Proceso", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 11, Productionorderid = "105", Salesorderid = "103", Status = "Cancelado", Userid = "abc", Comments = "Hello", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 12, Productionorderid = null, Salesorderid = "103", Status = "Finalizado", Userid = "abc", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 13, Productionorderid = "106", Salesorderid = "103", Status = "Finalizado", Userid = "abc", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 14, Productionorderid = null, Salesorderid = "104", Status = "Terminado", Userid = "abc", FinishDate = "29/08/2020" },
                new UserOrderModel { Id = 15, Productionorderid = "107", Salesorderid = "104", Status = "Terminado", Userid = "abc", FinishDate = "29/08/2020" },
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
        /// Gets the signature.
        /// </summary>
        /// <returns>the data.</returns>
        public List<UserOrderSignatureModel> GetSignature()
        {
            return new List<UserOrderSignatureModel>
            {
                new UserOrderSignatureModel { Id = 1000, LogisticSignature = null, TechnicalSignature = null, UserOrderId = 1 },
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
        public ResultModel GetResultModelGetFabricacionModelNoSerealize()
        {
            var listOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { DataSource = "O", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProductoId = "Aspirina", Quantity = 10, Status = "R" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelCompleteDetailModel()
        {
            var listDetalles = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 100, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "L" },
            };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 1, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L" },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L" },
                },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
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

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultUserModel()
        {
            var listUsers = new List<UserModel>
            {
                new UserModel { Activo = 1, FirstName = "Sutano", Id = "abc", LastName = "Lope", Password = "as", Role = 1, UserName = "sutan" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listUsers),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultAssignBatch()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-Aspirina-101", ServiceConstants.Ok },
                { "200-Aspirina-023", ServiceConstants.ErrorUpdateFabOrd },
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
        public ResultModel GetBatches()
        {
            var assigneBatches = new List<AssignedBatches>
            {
                new AssignedBatches { CantidadSeleccionada = 10, NumeroLote = "asd", SysNumber = 1 },
            };

            var listOrders = new List<BatchesComponentModel>
            {
                new BatchesComponentModel { CodigoProducto = "asd", LotesAsignados = assigneBatches },
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
        public ResultModel GetMissingBatches()
        {
            var listOrders = new List<BatchesComponentModel>
            {
                new BatchesComponentModel { CodigoProducto = "asd", LotesAsignados = new List<AssignedBatches>() },
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
        public ResultModel GetResultUpdateOrder()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-100", ServiceConstants.Ok },
                { "200-200", ServiceConstants.ErrorUpdateFabOrd },
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
        /// the values for the formulas.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetFormulaDetalle()
        {
            var listFormula = new CompleteFormulaWithDetalle { BaseDocument = 100, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 10, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 100, ProductDescription = "orden", ProductionOrderId = 100, ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo León, Mexico, CP. 54715", Comments = "Cooments" };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listFormula,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the result from detail orde rmodel.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetListCompleteDetailOrderModel()
        {
            var listDetails = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "CA", DescripcionProducto = "desc", FechaOf = "20/01/2020", FechaOfFin = "01/01/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "qfb", QtyPlanned = 100, QtyPlannedDetalle = 100, Status = "Planificado" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listDetails),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <returns>the users.</returns>
        public ResultModel GetUsersByRole()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "abc", Activo = 1, FirstName = "Gustavo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1" },
                new UserModel { Id = "abcd", Activo = 1, FirstName = "Hugo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = users,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Get new db context for in memory database.
        /// </summary>
        /// <param name="dbname">Data base name.</param>
        /// <returns>New context options.</returns>
        internal static DbContextOptions<DatabaseContext> CreateNewContextOptions(string dbname)
        {
            // Create a fresh service provider, and therefore a fresh.
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseInMemoryDatabase(dbname)
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
