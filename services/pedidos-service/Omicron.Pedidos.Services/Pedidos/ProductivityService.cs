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

            var userResponse = await this.userService.SimpleGetUsers(string.Format(ServiceConstants.GetUsersByRole, ServiceConstants.QfbRoleId));
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
            users = users.Where(x => x.Activo == 1).ToList();

            var userOrdersByDate = (await this.pedidosDao.GetUserOrderByFechaClose(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).ToList();

            var matrix = this.GetMatrix(dates, users, userOrdersByDate);
            return ServiceUtils.CreateResult(true, 200, null, matrix, null, null);
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
                var orderByUser = orders.Where(o => !string.IsNullOrEmpty(o.Userid) && o.Userid.Equals(u.Id)).ToList();
                var ordersId = orders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Productionorderid)).ToList();

                var sapResponse = await this.sapAdapter.PostSapAdapter(ordersId, ServiceConstants.GetUsersByOrdersById);
                var ordersSap = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(sapResponse.Response.ToString());

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
                listMonths.Add(culture.DateTimeFormat.GetMonthName(i));
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
                listToReturn.Add(total.ToString());
            }

            return listToReturn;
        }
    }
}
