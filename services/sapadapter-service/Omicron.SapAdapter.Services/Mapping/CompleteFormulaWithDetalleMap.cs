// <summary>
// <copyright file="CompleteFormulaWithDetalleMap.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Services.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Map methods for CompleteFormulaWithDetalleMap.
    /// </summary>
    public static class CompleteFormulaWithDetalleMap
    {
        /// <summary>
        /// Map properties from OrdenFabricacionModel to CompleteFormulaWithDetalle.
        /// </summary>
        /// <param name="self">Complete formula detail.</param>
        /// <param name="fabricationOrder">Fabrication order.</param>
        public static void Map(this CompleteFormulaWithDetalle self, OrdenFabricacionModel fabricationOrder)
        {
            fabricationOrder.PedidoId = fabricationOrder.PedidoId.HasValue ? fabricationOrder.PedidoId : 0;

            self.ProductionOrderId = fabricationOrder.OrdenId;
            self.Code = fabricationOrder.ProductoId;
            self.Type = ServiceUtils.GetDictionaryValueString(ServiceConstants.DictStatusType, fabricationOrder.Type, fabricationOrder.Type);
            self.Status = ServiceUtils.GetDictionaryValueString(ServiceConstants.DictStatus, fabricationOrder.Status, fabricationOrder.Status);
            self.PlannedQuantity = (int)fabricationOrder.Quantity;
            self.Unit = fabricationOrder.Unit;
            self.Warehouse = fabricationOrder.Wharehouse;
            self.Number = fabricationOrder.PedidoId.Value;
            self.FabDate = fabricationOrder.CreatedDate.Value.ToString("dd/MM/yyyy");
            self.DueDate = ServiceUtils.GetDateValueOrDefault(fabricationOrder.DueDate, string.Empty);
            self.StartDate = fabricationOrder.StartDate.ToString("dd/MM/yyyy");
            self.EndDate = ServiceUtils.GetDateValueOrDefault(fabricationOrder.PostDate, string.Empty);
            self.Origin = ServiceUtils.GetDictionaryValueString(ServiceConstants.DictStatusOrigin, fabricationOrder.OriginType, fabricationOrder.OriginType);
            self.BaseDocument = fabricationOrder.PedidoId.Value;
            self.Client = fabricationOrder.CardCode;
            self.CompleteQuantity = (int)fabricationOrder.CompleteQuantity;
        }

        /// <summary>
        /// Map properties from List CompleteDetalleFormulaModel to CompleteFormulaWithDetalle.
        /// </summary>
        /// <param name="self">Complete formula detail.</param>
        /// <param name="components">Components to set.</param>
        public static void Map(this CompleteFormulaWithDetalle self, List<CompleteDetalleFormulaModel> components)
        {
            self.HasBatches = components.Any(x => x.HasBatches);
            self.HasMissingStock = components.Any(y => y.Stock == 0);
            self.Details = components;
        }

        /// <summary>
        /// Map properties from UserOrderModel to CompleteFormulaWithDetalle.
        /// </summary>
        /// <param name="self">Complete formula detail.</param>
        /// <param name="userOrder">User order to map.</param>
        public static void Map(this CompleteFormulaWithDetalle self, UserOrderModel userOrder)
        {
            // Set defaults
            self.RealEndDate = string.Empty;
            self.Comments = string.Empty;

            if (userOrder != null)
            {
                self.Status = userOrder.Status;
                self.RealEndDate = ServiceUtils.GetDateValueOrDefault(userOrder.CloseDate, string.Empty);
                self.Comments = userOrder.Comments;
            }
        }
    }
}
