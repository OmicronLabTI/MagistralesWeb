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
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// class for the productivity services.
    /// </summary>
    public class ProductivityService : IProductivityService
    {
        private readonly IPedidosDao pedidosDao;

        private readonly IUsersService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductivityService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="userService">The user service.</param>
        public ProductivityService(IPedidosDao pedidosDao, IUsersService userService)
        {
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

            var userOrdersByDate = (await this.pedidosDao.GetUserOrderByFechaClose(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).Where(x => !x.IsIsolatedProductionOrder).ToList();

            var tupleRespond = this.GetMatrix(dates, users, userOrdersByDate);
            var matrix = this.OrderMatrix(tupleRespond.Item1, tupleRespond.Item2);

            var productivite = new ProductivityModel
            {
                Matrix = matrix,
            };

            return ServiceUtils.CreateResult(true, 200, null, productivite, null);
        }

        /// <summary>
        /// Gets the workload of the users.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetWorkLoad(Dictionary<string, string> parameters)
        {
            var users = await this.GetUsersByRole(ServiceConstants.QfbRoleId);
            var specificUser = string.Empty;
            if (parameters.ContainsKey(ServiceConstants.Qfb))
            {
                specificUser = parameters[ServiceConstants.Qfb];
                parameters.Remove(ServiceConstants.Qfb);
            }

            var dates = ServiceUtils.GetDateFilter(parameters);
            var finalizedOrders = (await this.pedidosDao.GetUserOrderByFechaClose(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).ToList();
            finalizedOrders = finalizedOrders.Where(x => x.IsProductionOrder && !x.IsIsolatedProductionOrder).ToList();

            // Método que buscar por planning date para obtener las ordenes por fecha de creación y reemplazar el SAP
            var userOrders = (await this.pedidosDao.GetUserOrderByPlanningDate(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).ToList();
            userOrders = userOrders.Where(x => !ServiceConstants.StatusIgnoreWorkLoad.Contains(x.Status)).ToList();
            userOrders.AddRange(finalizedOrders);
            userOrders = userOrders.Where(y => !y.IsIsolatedProductionOrder).DistinctBy(x => x.Id).ToList();

            var workLoad = this.GetWorkLoadByUser(users, userOrders, specificUser);
            return ServiceUtils.CreateResult(true, 200, null, workLoad, null);
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
        private Tuple<List<List<string>>, List<ProductiityTotalsModel>> GetMatrix(Dictionary<string, DateTime> dates, List<UserModel> users, List<UserOrderModel> orders)
        {
            var matrixToReturn = new List<List<string>>();
            var listProductivty = new List<ProductiityTotalsModel>();

            var valuesMonths = this.GetMonths(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin]);
            matrixToReturn.Add(valuesMonths.Item1);
            foreach (var u in users)
            {
                var orderByUser = orders.Where(o => !string.IsNullOrEmpty(o.Userid) && o.Userid.Equals(u.Id) && !o.IsIsolatedProductionOrder).ToList();
                var tupleResponse = this.GetDataByUser(u, orderByUser, valuesMonths.Item2);
                matrixToReturn.Add(tupleResponse.Item1);
                listProductivty.Add(tupleResponse.Item2);
            }

            return new Tuple<List<List<string>>, List<ProductiityTotalsModel>>(matrixToReturn, listProductivty);
        }

        /// <summary>
        /// Gets the list of months.
        /// </summary>
        /// <param name="initDate">the initDate.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        private Tuple<List<string>, List<int>> GetMonths(DateTime initDate, DateTime endDate)
        {
            var listMonths = new List<string>();
            var listNumMonths = new List<int>();
            listMonths.Add("-");

            var culture = new CultureInfo("es-MX");
            var currentMonth = culture.DateTimeFormat.GetMonthName(initDate.Month).ToUpper();
            var index = initDate.Month;

            var finalIndex = endDate.Month == 12 ? 1 : endDate.Month + 1;
            var finalMonth = culture.DateTimeFormat.GetMonthName(finalIndex).ToUpper();

            while (currentMonth != finalMonth)
            {
                listMonths.Add(culture.DateTimeFormat.GetMonthName(index).ToUpper());
                listNumMonths.Add(index);
                index += 1;
                index = index == 13 ? 1 : index;
                currentMonth = culture.DateTimeFormat.GetMonthName(index).ToUpper();
            }

            listMonths.Add("Total");

            return new Tuple<List<string>, List<int>>(listMonths, listNumMonths);
        }

        /// <summary>
        /// Gets the data by user.
        /// </summary>
        /// <param name="user">the user.</param>
        /// <param name="userOrder">the users userOrder.</param>
        /// <param name="months">The list of months.</param>
        /// <returns>the data.</returns>
        private Tuple<List<string>, ProductiityTotalsModel> GetDataByUser(UserModel user, List<UserOrderModel> userOrder, List<int> months)
        {
            var listToReturn = new List<string>();
            listToReturn.Add($"{user.FirstName} {user.LastName}");

            var totalPieces = 0;
            foreach (var i in months)
            {
                decimal total = 0;
                var monthNumber = i < 10 ? $"0{i}" : i.ToString();
                var userOrderByMonth = userOrder.Where(x => x.CloseDate.Value.ToString("dd/MM/yyyy").Contains($"/{monthNumber}/")).ToList();

                if (!userOrderByMonth.Any())
                {
                    listToReturn.Add("0");
                    continue;
                }

                total += userOrderByMonth.Sum(ts => ts.Quantity);
                totalPieces += (int)total;
                listToReturn.Add(((int)total).ToString());
            }

            listToReturn.Add(totalPieces.ToString());

            var listUsersMatrix = new ProductiityTotalsModel { Nombre = $"{user.FirstName} {user.LastName}", TotalPiezas = totalPieces };

            return new Tuple<List<string>, ProductiityTotalsModel>(listToReturn, listUsersMatrix);
        }

        /// <summary>
        /// Orders the matrix by total pieces.
        /// </summary>
        /// <param name="matrix">the matrix.</param>
        /// <param name="totals">the totals.</param>
        /// <returns>the data.</returns>
        private List<List<string>> OrderMatrix(List<List<string>> matrix, List<ProductiityTotalsModel> totals)
        {
            var listToReturn = new List<List<string>>();
            listToReturn.Add(matrix[0]);

            totals.OrderByDescending(x => x.TotalPiezas).ThenBy(x => x.Nombre).ToList().ForEach(x =>
            {
                var listByUser = matrix.FirstOrDefault(m => m[0].Equals(x.Nombre));
                listToReturn.Add(listByUser);
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the workload by user.
        /// </summary>
        /// <param name="users">the user.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="specificUser">the specific user to return.</param>
        /// <returns>the data.</returns>
        private List<WorkLoadModel> GetWorkLoadByUser(List<UserModel> users, List<UserOrderModel> userOrders, string specificUser)
        {
            if (!string.IsNullOrEmpty(specificUser))
            {
                return this.GetWorkloadSpecificUser(users, userOrders, specificUser);
            }

            return this.GetWorloadAllUsers(users, userOrders);
        }

        /// <summary>
        /// Gets the workload of a specific user.
        /// </summary>
        /// <param name="users">the list of users.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="specificUser">the specific user.</param>
        /// <returns>the data.</returns>
        private List<WorkLoadModel> GetWorkloadSpecificUser(List<UserModel> users, List<UserOrderModel> userOrders, string specificUser)
        {
            var user = users.FirstOrDefault(x => x.Id.Equals(specificUser));
            user = user == null ? new UserModel { Id = specificUser } : user;
            var ordersByUser = userOrders.Where(x => !string.IsNullOrEmpty(x.Userid) && x.Userid.Equals(user.Id)).ToList();
            var workLoad = this.GetTotalsByUser(ordersByUser, user);
            return new List<WorkLoadModel> { workLoad };
        }

        /// <summary>
        /// Gets the workload by all users.
        /// </summary>
        /// <param name="users">the list of users.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <returns>the data.</returns>
        private List<WorkLoadModel> GetWorloadAllUsers(List<UserModel> users, List<UserOrderModel> userOrders)
        {
            var listToReturn = new List<WorkLoadModel>();
            users.Where(x => x.Activo == 1).ToList().ForEach(user =>
            {
                var ordersByUser = userOrders.Where(x => !string.IsNullOrEmpty(x.Userid) && x.Userid.Equals(user.Id)).ToList();
                var workLoadByUser = this.GetTotalsByUser(ordersByUser, user);
                listToReturn.Add(workLoadByUser);
            });

            listToReturn = listToReturn.OrderByDescending(x => x.TotalPieces).ThenBy(x => x.User).ToList();

            listToReturn.Add(this.GetTotalAll(userOrders));
            return listToReturn;
        }

        /// <summary>
        /// Gets the total by user.
        /// </summary>
        /// <param name="usersOrders">the user orders.</param>
        /// <param name="user">the current user.</param>
        /// <returns>the data.</returns>
        private WorkLoadModel GetTotalsByUser(List<UserOrderModel> usersOrders, UserModel user)
        {
            var workLoadModel = new WorkLoadModel();
            workLoadModel.User = $"{user.FirstName} {user.LastName}";
            workLoadModel.TotalPossibleAssign = user.Piezas;

            workLoadModel = this.GetTotals(usersOrders, workLoadModel);
            return workLoadModel;
        }

        /// <summary>
        /// Get the total based on the user orders.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="workLoadModel">the workmodel.</param>
        /// <returns>the data.</returns>
        private WorkLoadModel GetTotals(List<UserOrderModel> userOrders, WorkLoadModel workLoadModel)
        {
            var productionOrders = userOrders.Where(x => x.IsProductionOrder && ServiceConstants.AllStatusWorkload.Contains(x.Status)).DistinctBy(x => x.Productionorderid).ToList();
            var productionOrderIds = productionOrders.Select(y => int.Parse(y.Productionorderid)).ToList();
            var salesOrderIds = productionOrders.Where(x => !string.IsNullOrEmpty(x.Salesorderid)).DistinctBy(x => x.Salesorderid).Select(y => int.Parse(y.Salesorderid)).ToList();
            int total = 0;
            ServiceConstants.StatusWorkload.ForEach(status =>
            {
                if (status == ServiceConstants.Finalizado)
                {
                    total = (int)productionOrders.Where(x => x.Status.Equals(status) || x.Status.Equals(ServiceConstants.Almacenado) || x.Status.Equals(ServiceConstants.Entregado)).Sum(ts => ts.Quantity);
                }
                else
                {
                    total = (int)productionOrders.Where(x => x.Status.Equals(status)).Sum(ts => ts.Quantity);
                }

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

            workLoadModel.TotalFabOrders = productionOrderIds.Count;
            workLoadModel.TotalOrders = salesOrderIds.Count;
            workLoadModel.TotalPieces = (int)productionOrders.DistinctBy(x => x.Productionorderid).Sum(y => y.Quantity);
            return workLoadModel;
        }

        /// <summary>
        /// Gets the total for all.
        /// </summary>
        /// <param name="userOrders">all the user orders.</param>
        /// <returns>the data.</returns>
        private WorkLoadModel GetTotalAll(List<UserOrderModel> userOrders)
        {
            var workLoadModel = new WorkLoadModel
            {
                User = "Total",
                TotalPossibleAssign = 0,
            };

            workLoadModel = this.GetTotals(userOrders, workLoadModel);
            return workLoadModel;
        }
    }
}
