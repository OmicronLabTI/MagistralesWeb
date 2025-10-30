// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Constants
{
    /// <summary>
    /// class ServiceConstants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Gets SuccessfulInvoiceCreationStatus.
        /// </summary>
        /// <value>
        /// String SuccessfulInvoiceCreationStatus.
        /// </value>
        public static string SuccessfulInvoiceCreationStatus => "Creación exitosa";

        /// <summary>
        /// Gets CreatingInvoiceStatus.
        /// </summary>
        /// <value>
        /// String CreatingInvoiceStatus.
        /// </value>
        public static string CreatingInvoiceStatus => "Creando factura";

        /// <summary>
        /// Gets InvoiceCreationErrorStatus.
        /// </summary>
        /// <value>
        /// String InvoiceCreationErrorStatus.
        /// </value>
        public static string InvoiceCreationErrorStatus => "Error al crear";
    }
}
