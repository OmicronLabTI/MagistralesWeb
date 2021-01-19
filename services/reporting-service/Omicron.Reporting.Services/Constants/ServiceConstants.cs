// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Constants
{
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
        /// the foreign.
        /// </summary>
        public const string ForeignEmailSubject = "Su pedido {0} de OmicronLab está en camino";

        /// <summary>
        /// The local.
        /// </summary>
        public const string InWayEmailSubject = "Su pedido {0} de OmicronLab está en camino";

        /// <summary>
        /// not delivered.
        /// </summary>
        public const string PackageNotDelivered = "Su pedido {0} de OmicronLab no pudo ser entregado";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string SendEmailHtmlBase = @"<html><body>{0}{1}{2}</body></html>";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string SentForeignPackage = "<p>Estimado Cliente.<br/><br/>Le informamos que su pedido <b>{0}</b> fue enviado por medio de la empresa de paquetería bajo la guía <b>{1}</b> a la dirección que tenemos registrada bajo su nombre.</p><p>Para conocer el estatus de su envío puede hacer click en el siguiente enlace:<br/><a href=\"http://www.dhl.com.mx/es/express.html\">http://www.dhl.com.mx/es/express.html</a></p>";

        /// <summary>
        /// Sent local package.
        /// </summary>
        public const string SentLocalPackage = "<p>Estimado Cliente.<br/><br/>Le informamos que su pedido <b>{0}</b> ya está planificado para ser entregado en la dirección que tenemos registrada bajo su nombre.</p>";

        /// <summary>
        /// not delivered body.
        /// </summary>
        public const string PackageNotDeliveredBody = "<p>Estimado cliente.<br/><br/>La entrega de su pedido <b>{0}</b> no pudo ser realizada. Le pedimos por favor que para programar una nueva visita se comunique:<ul><li>Con su asesor comercial</li><li>O al teléfono 81 15 22 2896 y/o al correo <u>atencion@o-lab.mx</u></li></ul></p>";

        /// <summary>
        /// Body of email.
        /// </summary>
        public const string FooterPayment = "<p>Para realizar el pago de este pedido le compartimos los datos para la transferencia:</p><p>OMICRONLAB S.A. DE C.V.<br/>Número de cuenta: 0515319189<br/>Banco: Banorte<br/>Clave Interbancaria: 072580005153191898<br/>Referencia: <b>{0}</b><br/>En caso de haber realizado el pago, haga caso omiso a esta información.</p>";

        /// <summary>
        /// refund policy.
        /// </summary>
        public const string RefundPolicy = "<p>*Política de Cambios y Devoluciones*</p><ul><li>Una vez recibido el pedido se otorgan 15 días calendario para notificar cualquier inconsistencia o inconformidad con su solicitud.</li><li>Para notificar cualquier incidencia favor enviar un correo a: <u>incidencias@o-lab.mx</u> con la siguiente información completa:<ul><li>PRODUCTO</li><li>CANTIDAD</li><li>LOTE O FECHA DE FABRICACIÓN</li><li>MOTIVO DE LA DEVOLUCIÓN</li></ul></li>Una vez recibida completa esta información, recibirá respuesta a la misma en un plazo máximo de 5 días hábiles. Si la información no está completa, la incidencia no procederá</ul><p>IMPORTATE: En ninguna circunstancia se aceptarán productos con evidencia de alteración o manipulación, tales como: re envasado, re etiquetado, o con alguna alteración visible en la integridad del producto.<br/><br/></p><center>Agradecemos su preferencia.</center><center>OmicronLab SA de CV</center>";
    }
}
