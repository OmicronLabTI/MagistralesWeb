// <summary>
// <copyright file="ProductivityService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// class for the productivity services.
    /// </summary>
    public class ProductivityService : IProductivityService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly IUsersService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductivityService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="userService">The user service.</param>
        public ProductivityService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, IUsersService userService)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Gets the productivity by users.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetProductivityData(Dictionary<string, string> parameters)
        {
            var dates = ServiceUtils.GetDateFilter(parameters);
            var users = await this.GetUsersByRole(ServiceConstants.QfbRoleId);
            users = users.Where(x => x.Activo == 1).ToList();

            var userOrdersByDate = (await this.pedidosDao.GetUserOrderByFechaClose(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).ToList();

            var matrix = await this.GetMatrix(dates, users, userOrdersByDate);
            var productivite = new ProductivityModel
            {
                Matrix = matrix,
            };

            return ServiceUtils.CreateResult(true, 200, null, productivite, null, null);
        }

        /// <summary>
        /// Gets the workload of the users.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetWorkLoad(Dictionary<string, string> parameters)
        {
            var users = await this.GetUsersByRole(ServiceConstants.QfbRoleId);

            var sapOrders = await this.GetSapFabOrders(parameters);
            var ordersId = sapOrders.Select(x => x.OrdenId.ToString()).ToList();
            var userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(ordersId)).ToList();

            var workLoad = this.GetWorkLoadByUser(users, userOrders, sapOrders);
            return ServiceUtils.CreateResult(true, 200, null, workLoad, null, null);
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <param name="role">the role to lookg.</param>
        /// <returns>the users.</returns>
        private async Task<List<UserModel>> GetUsersByRole(int role)
        {
            var userResponse = await this.userService.SimpleGetUsers(string.Format(ServiceConstants.GetUsersByRole, role));
            return JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the matrix value.
        /// </summary>
        /// <param name="dates">the dates.</param>
        /// <param name="users">all the users.</param>
        /// <param name="orders">the user orders from the active users..</param>
        /// <returns>the data.</returns>
        private async Task<List<List<string>>> GetMatrix(Dictionary<string, DateTime> dates, List<UserModel> users, List<UserOrderModel> orders)
        {
            var matrixToReturn = new List<List<string>>();

            matrixToReturn.Add(this.GetMonths(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin]));
            users = users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();

            foreach (var u in users)
            {
                var ordersSap = new List<FabricacionOrderModel>();
                var orderByUser = orders.Where(o => !string.IsNullOrEmpty(o.Userid) && o.Userid.Equals(u.Id)).ToList();
                var ordersId = orderByUser.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Productionorderid)).ToList();

                if (ordersId.Any())
                {
                    var sapResponse = await this.sapAdapter.PostSapAdapter(ordersId, ServiceConstants.GetUsersByOrdersById);
                    ordersSap = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(sapResponse.Response.ToString());
                }

                matrixToReturn.Add(this.GetDataByUser(u, orderByUser, ordersSap, dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin]));
            }

            return matrixToReturn;
        }

        /// <summary>
        /// Gets the list of months.
        /// </summary>
        /// <param name="initDate">the initDate.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        private List<string> GetMonths(DateTime initDate, DateTime endDate)
        {
            var listMonths = new List<string>();
            listMonths.Add(string.Empty);

            var culture = new CultureInfo("es-MX");
            for (var i = initDate.Month; i <= endDate.Month; i++)
            {
                listMonths.Add(culture.DateTimeFormat.GetMonthName(i).ToUpper());
            }

            return listMonths;
        }

        /// <summary>
        /// Gets the data by user.
        /// </summary>
        /// <param name="user">the user.</param>
        /// <param name="userOrder">the users userOrder.</param>
        /// <param name="fabOrder">the orders from sap.</param>
        /// <param name="initDate">the init date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        private List<string> GetDataByUser(UserModel user, List<UserOrderModel> userOrder, List<FabricacionOrderModel> fabOrder, DateTime initDate, DateTime endDate)
        {
            var listToReturn = new List<string>();
            listToReturn.Add($"{user.FirstName} {user.LastName}");

            for (var i = initDate.Month; i <= endDate.Month; i++)
            {
                decimal total = 0;
                var monthNumber = i < 10 ? $"0{i}" : i.ToString();
                var userOrderByMonth = userOrder.Where(x => x.CloseDate.Contains($"/{monthNumber}/")).ToList();

                if (!userOrderByMonth.Any())
                {
                    listToReturn.Add("0");
                    continue;
                }

                var userOrderIds = userOrderByMonth.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Productionorderid)).ToList();
                var orderFromSap = fabOrder.Where(x => userOrderIds.Contains(x.OrdenId)).ToList();
                total += orderFromSap.Sum(x => x.Quantity);
                listToReturn.Add(((int)total).ToString());
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the workload by user.
        /// </summary>
        /// <param name="users">the user.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="sapOrders">the sap order.</param>
        /// <returns>the data.</returns>
        private List<WorkLoadModel> GetWorkLoadByUser(List<UserModel> users, List<UserOrderModel> userOrders, List<FabricacionOrderModel> sapOrders)
        {
            var listToReturn = new List<WorkLoadModel>();
            users.Where(x => x.Activo == 1).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList().ForEach(user =>
            {
                var ordersByUser = userOrders.Where(x => !string.IsNullOrEmpty(x.Userid) && x.Userid.Equals(user.Id)).ToList();
                var workLoadByUser = this.GetTotalsByUser(ordersByUser, sapOrders, user);
                listToReturn.Add(workLoadByUser);
            });

            listToReturn.Add(this.GetTotalAll(userOrders, sapOrders));
            return listToReturn;
        }

        /// <summary>
        /// Gets the total by user.
        /// </summary>
        /// <param name="usersOrders">the user orders.</param>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="user">the current user.</param>
        /// <returns>the data.</returns>
        private WorkLoadModel GetTotalsByUser(List<UserOrderModel> usersOrders, List<FabricacionOrderModel> sapOrders, UserModel user)
        {
            var workLoadModel = new WorkLoadModel();
            workLoadModel.User = $"{user.FirstName} {user.LastName}";
            workLoadModel.TotalPossibleAssign = 200;

            workLoadModel = this.GetTotals(usersOrders, sapOrders, workLoadModel);
            return workLoadModel;
        }

        /// <summary>
        /// Get the total based on the user orders.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="workLoadModel">the workmodel.</param>
        /// <returns>the data.</returns>
        private WorkLoadModel GetTotals(List<UserOrderModel> userOrders, List<FabricacionOrderModel> sapOrders, WorkLoadModel workLoadModel)
        {
            ServiceConstants.StatusWorkload.ForEach(status =>
            {
                var ordersByStatus = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Status.Equals(status)).Select(y => int.Parse(y.Productionorderid)).ToList();
                var total = (int)sapOrders.Where(x => ordersByStatus.Any(y => y == x.OrdenId)).Sum(y => y.Quantity);

                switch (status)
                {
                    case ServiceConstants.Asignado:
                        workLoadModel.Assigned = total;
                        break;

                    case ServiceConstants.Proceso:
                        workLoadModel.Processed = total;
                        break;

                    case ServiceConstants.Pendiente:
                        workLoadModel.Pending = total;
                        break;

                    case ServiceConstants.Terminado:
                        workLoadModel.Finished = total;
                        break;

                    case ServiceConstants.Finalizado:
                        workLoadModel.Finalized = total;
                        break;

                    case ServiceConstants.Reasignado:
                        workLoadModel.Reassigned = total;
                        break;
                }
            });

            workLoadModel.TotalFabOrders = userOrders.DistinctBy(y => y.Productionorderid).ToList().Count;
            workLoadModel.TotalOrders = userOrders.DistinctBy(y => y.Salesorderid).ToList().Count;

            var ordersId = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Productionorderid)).ToList();
            workLoadModel.TotalPieces = sapOrders.Where(x => ordersId.Any(y => y == x.OrdenId)).Sum(y => (int)y.Quantity);

            return workLoadModel;
        }

        /// <summary>
        /// Gets the total for all.
        /// </summary>
        /// <param name="userOrders">all the user orders.</param>
        /// <param name="sapOrders">all the sap orders.</param>
        /// <returns>the data.</returns>
        private WorkLoadModel GetTotalAll(List<UserOrderModel> userOrders, List<FabricacionOrderModel> sapOrders)
        {
            var workLoadModel = new WorkLoadModel
            {
                User = "Total",
                TotalPossibleAssign = 0,
            };

            workLoadModel = this.GetTotals(userOrders, sapOrders, workLoadModel);
            return workLoadModel;
        }

        /// <summary>
        /// Gets the sap fab orders.
        /// </summary>
        /// <param name="parameters">the dict.</param>
        /// <returns>the data.</returns>
        private async Task<List<FabricacionOrderModel>> GetSapFabOrders(Dictionary<string, string> parameters)
        {
            var listToReturn = new List<FabricacionOrderModel>();
            parameters.Add(ServiceConstants.Offset, "0");
            parameters.Add(ServiceConstants.Limit, "8000");
            var offset = 0;
            int total;

            do
            {
                parameters[ServiceConstants.Offset] = offset.ToString();
                var sapResponse = await this.sapAdapter.PostSapAdapter(new GetOrderFabModel { Filters = parameters, OrdersId = new List<int>() }, ServiceConstants.GetFabOrdersByFilter);
                listToReturn.AddRange(JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(sapResponse.Response.ToString()));

                total = sapResponse.Comments != null ? int.Parse(sapResponse.Comments.ToString()) : 0;
                offset += 8000;
            }
            while (total > 0 && offset < total);

            return listToReturn;
        }
    }
}
