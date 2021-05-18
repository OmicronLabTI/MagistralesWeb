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
    using System.Text;
    using Omicron.SapAdapter.Entities.Model;
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
        /// <param name="orderHeaders">the orders header.</param>
        /// <param name="lineProducts">the lines products.</param>
        /// <returns>the datetime.</returns>
        public static List<CompleteAlmacenOrderModel> GetSapOrderByType(List<string> types, List<CompleteAlmacenOrderModel> sapOrders, List<OrdenFabricacionModel> orderHeaders, List<string> lineProducts)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();
            var sapOrdersGroup = sapOrders.GroupBy(x => x.DocNum).ToList();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => x.Count() == orderHeaders.Where(y => y.PedidoId == x.Key).Count());
                var keys = listMagistral.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keys.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => x.Count() != orderHeaders.Where(y => y.PedidoId == x.Key).Count() && orderHeaders.Where(y => y.PedidoId == x.Key).Count() > 0);
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => orderHeaders.Where(y => y.PedidoId == x.Key).Count() == 0 && x.All(y => lineProducts.Contains(y.Detalles.ProductoId)));
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Maquila.ToLower()))
            {
                var ordersMaquila = sapOrders.Where(x => x.TypeOrder == ServiceConstants.OrderTypeMQ).ToList();
                var orderListToAdd = new List<CompleteAlmacenOrderModel>();
                foreach (var order in ordersMaquila)
                {
                    var orderExists = listToReturn.FirstOrDefault(x => x.DocNum == order.DocNum && x.Detalles.ProductoId == order.Detalles.ProductoId);
                    if (orderExists == null)
                    {
                        orderListToAdd.Add(order);
                    }
                }

                listToReturn.AddRange(orderListToAdd);
            }
            else
            {
                listToReturn = listToReturn.Where(x => x.TypeOrder != ServiceConstants.OrderTypeMQ).ToList();
            }

            if (types.Contains(ServiceConstants.Muestra.ToLower()))
            {
                var ordersMuestra = sapOrders.Where(x => !string.IsNullOrEmpty(x.PedidoMuestra) && x.PedidoMuestra == ServiceConstants.IsSampleOrder).ToList();
                var orderListToAdd = new List<CompleteAlmacenOrderModel>();
                foreach (var order in ordersMuestra)
                {
                    var orderExists = listToReturn.FirstOrDefault(x => x.DocNum == order.DocNum && x.Detalles.ProductoId == order.Detalles.ProductoId);
                    if (orderExists == null)
                    {
                        orderListToAdd.Add(order);
                    }
                }

                listToReturn.AddRange(orderListToAdd);
            }
            else
            {
                listToReturn = listToReturn.Where(x => string.IsNullOrEmpty(x.PedidoMuestra) || x.PedidoMuestra != ServiceConstants.IsSampleOrder).ToList();
            }

            return listToReturn;
        }
    }
}
