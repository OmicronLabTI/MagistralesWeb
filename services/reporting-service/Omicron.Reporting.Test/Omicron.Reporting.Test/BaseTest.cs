// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test
{
    using System.Collections.Generic;
    using System.IO;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Create mock production orders.
        /// </summary>
        /// <returns>Mock users.</returns>
        public RawMaterialRequestModel GetMockRawMaterialRequest()
        {
            var request = new RawMaterialRequestModel
            {
                Id = 1,
                Observations = "Observaciones",
                Signature = File.ReadAllText("SignatureBase64.txt"),
                SigningUserName = "Nombre Usuario Firma",
                ProductionOrderIds = new List<int> { 1, 2, 3, 4 },
                OrderedProducts = new List<RawMaterialRequestDetailModel>
                {
                    new RawMaterialRequestDetailModel
                    {
                        ProductId = "ProdI",
                        Description = "Producto I",
                        RequestQuantity = 11.2M,
                        Unit = "Kilogramos",
                    },
                    new RawMaterialRequestDetailModel
                    {
                        ProductId = "ProdII",
                        Description = "Producto II",
                        RequestQuantity = 12.2M,
                        Unit = "Litros",
                    },
                },
            };

            return request;
        }
    }
}
