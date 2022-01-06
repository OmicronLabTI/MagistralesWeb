// <summary>
// <copyright file="ServiceUtilsAlmacen.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;

    /// <summary>
    /// The class for the services.
    /// </summary>
    public static class ServiceUtilsAlmacen
    {
        /// <summary>
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="types">the type to filter.</param>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="lineProducts">the lines products.</param>
        /// <returns>the datetime.</returns>
        public static Tuple<List<CompleteAlmacenOrderModel>, SaleOrderTypeModel> GetSapOrderByType(List<string> types, List<CompleteAlmacenOrderModel> sapOrders, List<string> lineProducts)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();
            var salesTypes = new SaleOrderTypeModel
            {
                LineSaleOrders = new List<int>(),
                MagistralSaleOrders = new List<int>(),
                MixedSaleOrders = new List<int>(),
            };

            var sapOrdersGroup = sapOrders.GroupBy(x => x.DocNum).ToList();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => !x.Any(y => lineProducts.Contains(y.Detalles.ProductoId)) && !x.All(y => lineProducts.Contains(y.Detalles.ProductoId)));
                var keys = listMagistral.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keys.Contains(x.DocNum)));
                salesTypes.MagistralSaleOrders = keys;
                sapOrdersGroup.RemoveAll(x => keys.Contains(x.Key));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => x.Any(y => lineProducts.Contains(y.Detalles.ProductoId) && !x.All(y => lineProducts.Contains(y.Detalles.ProductoId))));
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
                salesTypes.MixedSaleOrders = keysMixta;
                sapOrdersGroup.RemoveAll(x => keysMixta.Contains(x.Key));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => x.All(y => lineProducts.Contains(y.Detalles.ProductoId)));
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
                salesTypes.LineSaleOrders = keysMixta;
                sapOrdersGroup.RemoveAll(x => keysMixta.Contains(x.Key));
            }

            if (!types.Contains(ServiceConstants.Maquila.ToLower()))
            {
                listToReturn = listToReturn.Where(x => x.TypeOrder != ServiceConstants.OrderTypeMQ).ToList();
            }

            if (!types.Contains(ServiceConstants.Muestra.ToLower()))
            {
                listToReturn = listToReturn.Where(x => string.IsNullOrEmpty(x.PedidoMuestra) || x.PedidoMuestra != ServiceConstants.IsSampleOrder).ToList();
            }

            return new Tuple<List<CompleteAlmacenOrderModel>, SaleOrderTypeModel>(listToReturn.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList(), salesTypes);
    }

        /// <summary>
        /// Get the orders for recepcion pedidos.
        /// </summary>
        /// <param name="sapDao">dao.</param>
        /// <param name="userOrdersTuple">user order tuuple.</param>
        /// <param name="lineProductTuple">line produc tuple.</param>
        /// <returns>the orders.</returns>
        public static async Task<List<CompleteAlmacenOrderModel>> GetSapOrderForRecepcionPedidos(ISapDao sapDao, Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple)
        {
            var idsMagistrales = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();

            var sapOrders = (await sapDao.GetAllOrdersForAlmacen(userOrdersTuple.Item3)).ToList();
            sapOrders = sapOrders.Where(x => x.Detalles != null).ToList();
            var arrayOfSaleToProcess = new List<CompleteAlmacenOrderModel>();

            sapOrders.Where(o => o.Canceled == "N").GroupBy(x => x.DocNum).ToList().ForEach(x =>
            {
                if (x.All(y => y.IsMagistral == "Y") && idsMagistrales.Contains(x.Key))
                {
                    arrayOfSaleToProcess.AddRange(x.ToList());
                }
                else if (x.All(y => y.IsLine == "Y") && !lineProductTuple.Item2.Contains(x.Key))
                {
                    arrayOfSaleToProcess.AddRange(x.ToList());
                }
                else if (x.Any(y => y.IsLine == "Y") && x.Any(y => y.IsMagistral == "Y") && idsMagistrales.Contains(x.Key))
                {
                    arrayOfSaleToProcess.AddRange(x.ToList());
                }
            });

            arrayOfSaleToProcess.AddRange(sapOrders.Where(o => o.Canceled == "Y"));
            var orderToAppear = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).ToList();
            var ordersSapMaquila = (await sapDao.GetAllOrdersForAlmacenByTypeOrder(ServiceConstants.OrderTypeMQ, orderToAppear)).ToList();
            arrayOfSaleToProcess.AddRange(ordersSapMaquila.Where(x => x.Detalles != null));

            arrayOfSaleToProcess = arrayOfSaleToProcess.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList();
            return arrayOfSaleToProcess;
        }
    }
}
