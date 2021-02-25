// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Constants
{
    using System.Collections.Generic;

    /// <summary>
    /// the class for constatns.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Status of planed production orders.
        /// </summary>
        public const string ProductionOrderPlannedStatus = "Planificado";

        /// <summary>
        /// Pattern of raw material request file name.
        /// </summary>
        public const string RawMaterialRequestFileNamePattern = "Solicitud_MP_{0}.pdf";

        /// <summary>
        /// Subject of email.
        /// </summary>
        public const string RawMaterialRequestEmailSubject = "Solicitud de Materia Prima";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string RawMaterialRequestEmailBody = "Se ha enviado una solicitud de Materia Prima, favor de no responder este correo.";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string CustomerServiceEmail = "EmailAtencionAClientes";

        /// <summary>
        /// The local.
        /// </summary>
        public const string InRejectedEmailSubject = "Aviso de rechazo Orden de Venta {0}; {1}";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentRejectedOrder = " <p> Buen día, es un gusto saludarle <br/><br/> Hemos detectado que la orden de venta <b>{0}</b> correspondiente al cliente <b>{1}</b> incumple con los requisitos obligatorios para poder comenzar su elaboración. <br/><br/>  </p>";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentComentRejectedOrder = " <p> <b>Observaciones:</b> <br/><br/> <b>{0}</b> <br/><br/> </p>";

        /// <summary>
        /// refund policy.
        /// </summary>
        public const string EmailRejectedOrderClosing = " Quedamos atentos a sus comentarios. <br/> Departamento Magistrales";

        /// <summary>
        /// refund policy.
        /// </summary>
        public const string EmailFarewall = "<p> Le pedimos de la manera más atenta tomar la acción correspondiente.<br/> Agradecemos de antemano su seguimiento, y pedimos una disculpa por los inconvenientes ocasionados.</p>";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string SendEmailHtmlBase = @"<html><body>{0}{1}{2}{3}</body></html>";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ValuesForEmail { get; } = new List<string>
        {
          "EmailMiddleware",
          "EmailMiddlewarePassword",
          "SmtpServer",
          "SmtpPort",
          "EmailCCDelivery",
        };
    }
}
