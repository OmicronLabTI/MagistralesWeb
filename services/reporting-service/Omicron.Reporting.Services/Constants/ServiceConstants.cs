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
        public const string DelPartyEmail = "Email";

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
        /// the foreign.
        /// </summary>
        public const string ForeignEmailSubject = "Su pedido {0} de OmicronLab está en camino";

        /// <summary>
        /// The local.
        /// </summary>
        public const string InWayEmailSubject = "Su pedido {0} de OmicronLab está en camino";

        /// <summary>
        /// The local.
        /// </summary>
        public const string DeliveryEmailSubject = "Su pedido {0} se entregó con éxito";

        /// <summary>
        /// not delivered.
        /// </summary>
        public const string PackageNotDelivered = "Su pedido {0} de OmicronLab no pudo ser entregado";

        /// <summary>
        /// not delivered.
        /// </summary>
        public const string PaqueteEmail = "Para conocer el estatus de su envío puede hacer click en el siguiente enlace:";

        /// <summary>
        /// not delivered.
        /// </summary>
        public const string TelefonoEmail = "Para conocer el estatus de su envío comunicate al siguiente número teléfonico:";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string SendEmailHtmlBaseAlmacen = @"<html><header>{0}</header><body>{1}{2}{3}</body></html>";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string PlaceLink = "<a href=\"{0}\">{0}</a>";

        /// <summary>
        /// Spei notification footer.
        /// </summary>
        public const string LogoMailHeader = "<div align=\"center\"><img data-imagetype=\"External\" src = \"{0}\" alt=\"OmicronLab\" style=\"width:auto; height:auto\" ></div>";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string CustomerServiceEmail = "EmailAtencionAClientes";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string EmailCCRejected = "EmailCCRejected";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string LogisticEmailCc2Field = "EmailLogisticaCc2";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string LogisticEmailCc3Field = "EmailLogisticaCc3";

        /// <summary>
        /// Get the email logo url.
        /// </summary>
        public const string EmailLogoUrl = "EmailLogoUrl";

        /// <summary>
        /// The local.
        /// </summary>
        public const string InRejectedEmailSubject = "Aviso de rechazo Orden de Venta {0}; {1}";

        /// <summary>
        /// The local.
        /// </summary>
        public const string InCancelDeliveryEmailSubject = "La Entrega {0} fue cancelada.";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentCancelDelivery = "<p>Estimado Colaborador.<br/><br/>Le informamos que la entrega <b>{0}</b> la cual contiene el(los) pedido(s) <b>{1}</b> fue cancelada por almacén.</p>";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentRejectedOrder = " <p> Buen día, es un gusto saludarle <br/><br/> Hemos detectado que la orden de venta <b>{0}</b> correspondiente al cliente <b>{1}</b> incumple con los requisitos obligatorios para poder comenzar su elaboración.<br/><br/></p>";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentForeignPackage = "<p>Estimado Cliente.<br/><br/>Le informamos que el pedido <b>{0}</b> ha sido enviado por medio de paquetería a la dirección registrada bajo su nombre, el número de guía correspondiente es <b>{1}</b>.</p><p>{2}<br/>{3}</p>";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentLocalPackage = "<p>Estimado Cliente.<br/><br/>Le informamos que el pedido <b>{0}</b> se encuentra en ruta para ser entregado en la dirección registrada a su nombre. Si desea consultar su pedido puede hacerlo aquí:</p>{1}<br/>";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentLocalPackageDelivery = "<p>Estimado Cliente.<br/><br/>Le informamos que el pedido <b>{0}</b> fue entregado con éxito.</p><p>Si desea consultar su pedido puede hacerlo aquí:</p>{1}<br/>";

        /// <summary>
        /// not delivered body.
        /// </summary>
        public const string PackageNotDeliveredBody = "<p>Estimado cliente.<br/><br/>La entrega del pedido <b>{0}</b> no pudo ser realizada. Para programar una nueva visita le pedimos de la manera más atenta se comunique:<ul><li>Con su asesor comercial</li><li>O al teléfono 81 15 22 2896 y/o al correo <u>atencion@o-lab.mx</u></li></ul></p><p>Si desea consultar su pedido puede hacerlo aquí:</p>{1}<br/>";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string FooterPayment = "<p>Para realizar el pago de este pedido le compartimos los datos para la transferencia:</p><p>OMICRONLAB S.A. DE C.V.<br/><br/>Banco: Banorte<br/>Número de cuenta: 0515319189<br/>Clave Interbancaria: 072580005153191898<br/>Referencia: <b>{0}</b><br/><br/>Banco: Bancomer<br/>Número de cuenta: 0116534406<br/>Clave Interbancaria: 012580001165344066<br/>Referencia: <b>{0}</b><br/><br/>En caso de haber realizado el pago, haga caso omiso a esta información.</p>";

        /// <summary>
        /// refund policy.
        /// </summary>
        public const string RefundPolicy = "<p>*Política de Cambios y Devoluciones*</p><ul><li>Una vez recibido el pedido se otorgan 15 días calendario para notificar cualquier inconsistencia o inconformidad con su solicitud.</li><li>Para notificar cualquier incidencia favor enviar un correo a: <u>incidencias@o-lab.mx</u>; por WhatsApp al número:  8118106776 o a través de su plataforma.</li></ul><center>Agradecemos su preferencia.</center><center>OmicronLab SA de CV</center>";

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
        /// refund policy.
        /// </summary>
        public const string EmailFarewallCancelDelivery = "<p>Agradecemos así mismo que nos puedan apoyar con la cancelación del(los) pedido(s) para concluir con el proceso.</p>";

        /// <summary>
        /// refund policy.
        /// </summary>
        public const string EmailCancelDeliveryClosing = "<center>Agradecemos de antemano su atención.</center><center>ALMACÉN PT</center><center>OmicronLab SA de CV</center>";

        /// <summary>
        /// <summary>
        /// Body of email.
        /// </summary>
        public const string SendEmailHtmlBase = @"<html><body>{0}{1}{2}{3}</body></html>";

        /// <summary>
        /// Paynet hiper link paynet recipe.
        /// </summary>
        public const string ButtonEmail = "<table align=\"left\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" role=\"presentation\"><tbody><tr><td class=\"x_fallback-font x_o_btn x_o_bg-primary x_o_br x_o_heading x_o_text\" align=\"center\" style=\"font-family: &quot;Open Sans&quot;, Arial, Helvetica, sans-serif; font-weight: bold; margin-top: 0px; margin-bottom: 0px; font-size: 16px; line-height: 24px; background-color: rgb(51, 128, 255) !important; color: rgb(51, 128, 255); border-radius: 4px;\" data-ogsb=\"rgb(51, 128, 255)\"><a href =\"{0} \" target=\"_blank\" rel=\"noopener noreferrer\" data-auth=\"NotApplicable\" class=\"x_o_text-white\" style=\"text-decoration:none; outline:none; color:#f8f8f8; display:block; padding:12px 24px\" data-linkindex=\"2\">Entrar</a></td></tr></tbody></table></br>";

        /// <summary>
        /// The enviado status.
        /// </summary>
        public const string Entregado = "Entregado";

        /// <summary>
        /// the incidentFilename.
        /// </summary>
        public const string IncidentFileName = "IncidenciasInternasPT-";

        /// <summary>
        /// email for the excel for incidents.
        /// </summary>
        public const string EmailIncidentReport = "EmailIncidentReport";

        /// <summary>
        /// subject for email.
        /// </summary>
        public const string SubjectIncidentReport = "Incidencias internas almacén PT {0}";

        /// <summary>
        /// subject for email.
        /// </summary>
        public const string BodyIncidentReport = "<p> Se adjunta el reporte semanal de Incidencias internas del almacén PT creadas del {0} al {1}.";

        /// <summary>
        /// refund policy.
        /// </summary>
        public const string ReportIncidentClosing = "<p> <b>Resumen:</b> </p> {0} </p> Favor de no responder este correo. </p>";

        /// <summary>
        /// <summary>
        /// Body of email.
        /// </summary>
        public const string SendEmailIncidentHtmlBase = @"<html><body>{0}{1}</body></html>";

        /// <summary>
        /// subject for email.
        /// </summary>
        public const string IncidentDataTableName = "Incidencias";

        /// <summary>
        /// bioequal email.
        /// </summary>
        public const string EmailBioEqual = "EmailBioEqual";

        /// <summary>
        /// bioelite email.
        /// </summary>
        public const string EmailBioElite = "EmailBioElite";

        /// <summary>
        /// bioelite email.
        /// </summary>
        public const string BioEqual = "BE";

        /// <summary>
        /// bioelite email.
        /// </summary>
        public const string BioElite = "MN";

        /// <summary>
        /// bioelite email.
        /// </summary>
        public const string Mixture = "MX";

        /// <summary>
        /// comments.
        /// </summary>
        public const string Comments = "Comments";

        /// <summary>
        /// Create date.
        /// </summary>
        public const string CreateDate = "CreateDateString";

        /// <summary>
        /// Create date.
        /// </summary>
        public const string Mixto = "Mixto";

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

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> IncidentKeys { get; } = new Dictionary<string, string>
        {
            { "ID", "Id" },
            { "FECHA INCIDENCIA", CreateDate },
            { "PEDIDO", "SaleOrder" },
            { "REMISIÓN", "Delivery" },
            { "FACTURA", "Invoice" },
            { "PRODUCTO", "ItemCode" },
            { "TIPO", "Type" },
            { "TIPO DE FALLA", "Incident" },
            { "LOTE / PIEZAS DAÑADAS", "Batches" },
            { "ETAPA", "Stage" },
            { "ESTATUS INCIDENCIA", "Status" },
            { "COMENTARIOS", Comments },
        };
    }
}
