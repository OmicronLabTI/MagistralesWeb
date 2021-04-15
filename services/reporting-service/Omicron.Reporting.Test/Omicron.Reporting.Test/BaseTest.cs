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

        /// <summary>
        /// Create mock signature.
        /// </summary>
        /// <returns>the signature.</returns>
        public string GetSignatureExample()
        {
            var signature = "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";

            return signature;
        }

        /// <summary>
        /// Create smtp config.
        /// </summary>
        /// <returns>the smtp config.</returns>
        public SmtpConfigModel GetSMTPConfig()
        {
            var smtpConfig = new SmtpConfigModel
            {
                SmtpServer = "192.168.0.1",
                SmtpPort = 5434,
                SmtpDefaultPassword = "EmailMiddlewarePassword",
                SmtpDefaultUser = "EmailUser",
                EmailCCDelivery = "EmailCCDelivery",
            };
            return smtpConfig;
        }

        /// <summary>
        /// Create smtp config.
        /// </summary>
        /// <returns>the smtp config.</returns>
        public RawMaterialEmailConfigModel GetRawMaterialEmailConfigModel()
        {
            var rawMaterialConfig = new RawMaterialEmailConfigModel
            {
                Addressee = "EmailAlmacen@email.com",
                CopyTo = "copy@email.com",
            };
            return rawMaterialConfig;
        }
    }
}
