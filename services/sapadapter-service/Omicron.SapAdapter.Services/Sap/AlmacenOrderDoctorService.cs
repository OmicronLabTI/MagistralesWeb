// <summary>
// <copyright file="AlmacenOrderDoctorService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for almacen order doctor service.
    /// </summary>
    public class AlmacenOrderDoctorService : IAlmacenOrderDoctorService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDoctorService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        public AlmacenOrderDoctorService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersByDoctor(Dictionary<string, string> parameters)
        {
            var doctorValue = parameters.ContainsKey(ServiceConstants.Doctor) ? parameters[ServiceConstants.Doctor].Split(",").ToList() : new List<string>();

            // get user orders
            var userOrdersTuple = await this.GetUserOrders();
            return ServiceUtils.CreateResult(true, 200, null, "respuest", null, "comments");
        }

        private async Task<Tuple<List<UserOrderModel>, List<int>, DateTime>> GetUserOrders()
        {
            var userOrderModel = await this.pedidosService.GetUserPedidos(ServiceConstants.GetUserOrdersAlmancen);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());

            var listIds = JsonConvert.DeserializeObject<List<int>>(userOrderModel.ExceptionMessage);

            int.TryParse(userOrderModel.Comments.ToString(), out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));

            return new Tuple<List<UserOrderModel>, List<int>, DateTime>(userOrders, listIds, dateToLook);
        }
    }
}
