// <summary>
// <copyright file="QueryableExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DataAccess.Extensions
{
    using System.Linq;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.Wraps;

    /// <summary>
    /// QueryableExtension class.
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// Method to get complete order
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Complete order.</returns>
        public static IQueryable<CompleteOrderModel> GetCompleteOrdery(this IQueryable<CompleteOrderModelWrap> source)
        {
            return source.Select(s => new CompleteOrderModel
            {
                DocNum = s.OrderModel.DocNum,
                Cliente = s.ClientCatalogModel.AliasName,
                Codigo = s.OrderModel.Codigo,
                Medico = s.ClientCatalogModel.AliasName,
                AsesorName = s.AsesorModel.AsesorName,
                FechaInicio = s.OrderModel.FechaInicio.ToString("dd/MM/yyyy"),
                FechaFin = s.OrderModel.FechaFin.ToString("dd/MM/yyyy"),
                PedidoStatus = s.OrderModel.PedidoStatus,
                AtcEntry = s.OrderModel.AtcEntry,
                IsChecked = false,
                Detalles = s.DetallePedidoModel,
                OrderType = s.OrderModel.OrderType,
                Canceled = s.OrderModel.Canceled,
                PedidoMuestra = s.OrderModel.PedidoMuestra,
                DocNumDxp = s.OrderModel.DocNumDxp,
                ShippingAddressName = s.OrderModel.ShippingAddressName,
                ClientType = s.OrderModel.ClientType,
            });
        }
    }
}
