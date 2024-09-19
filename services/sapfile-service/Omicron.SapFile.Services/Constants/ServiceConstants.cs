// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.SapFile.Services.Utils;
using System.Collections.Generic;

namespace Omicron.SapFile.Services.Constants
{
    /// <summary>
    /// the service constants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Text for sales order reference.
        /// </summary>
        public const string SalesOrderReferenceText = "Pedido: {0}";

        /// <summary>
        /// Text for production order reference.
        /// </summary>
        public const string ProductionOrderReferenceText = "Orden de fabricación: {0}";

        /// <summary>
        /// Text for production order reference.
        /// </summary>
        public const string SigantureLabel = "Firma etiqueta";

        /// <summary>
        /// Text for production order reference.
        /// </summary>
        public const string GenericSignature = "genérica";

        /// <summary>
        /// Text for production order reference.
        /// </summary>
        public const string PersonalizedSignature = "personalizada";

        /// <summary>
        /// Text for production order reference.
        /// </summary>
        public const string Invoice = "invoice";

        /// <summary>
        /// Text for production order reference.
        /// </summary>
        public const string Delivery = "delivery";

        /// <summary>
        /// Azure Key.
        /// </summary>
        public const string AzureKey = "AzureKey";

        /// <summary>
        /// Azure Account Name.
        /// </summary>
        public const string AzureAccountName = "AzureAccountName";


        /// <summary>
        /// Character Path Separator.
        /// </summary>
        public const char CharacterPathSeparator = '/';

        /// <summary>
        /// Prescription Files.
        /// </summary>
        public const string PrescriptionFiles = "PrescriptionFiles";

        /// <summary>
        /// Azure Account Name.
        /// </summary>
        public const string InstitutionalClientType = "institucional";
    }
}
