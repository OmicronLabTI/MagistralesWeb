// <summary>
// <copyright file="SapCreateSaleOrder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using Omicron.SapDiApi.Entities.Context;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Entities.Models.Experience;
    using Omicron.SapDiApi.Log;
    using SAPbobsCOM;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// class create order.
    /// </summary>
    public class SapCreateSaleOrder : ISapCreateSaleOrder
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapCreateSaleOrder"/> class.
        /// </summary>   
        public SapCreateSaleOrder(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateSaleOrder(CreateSaleOrderModel saleOrderModel)
        {

        }
    }
}
