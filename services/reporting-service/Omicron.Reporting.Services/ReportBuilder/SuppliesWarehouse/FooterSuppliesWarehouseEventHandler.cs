// <summary>
// <copyright file="FooterSuppliesWarehouseEventHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services.ReportBuilder.SuppliesWarehouse
{
    using System;
    using System.IO;
    using iText.IO.Image;
    using iText.Kernel.Colors;
    using iText.Kernel.Events;
    using iText.Kernel.Font;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas;
    using iText.Layout;
    using iText.Layout.Borders;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// HeaderSuppliesWarehouseEventHandler class.
    /// </summary>
    public class FooterSuppliesWarehouseEventHandler : IEventHandler
    {
        private readonly RawMaterialRequestModel request;
        private readonly Color signatureColor = new DeviceRgb(237, 237, 237);
        private readonly Color textColor = new DeviceRgb(37, 37, 37);

        /// <summary>
        /// Initializes a new instance of the <see cref="FooterSuppliesWarehouseEventHandler"/> class.
        /// </summary>
        /// <param name="request">Request.</param>
        public FooterSuppliesWarehouseEventHandler(RawMaterialRequestModel request)
        {
            this.request = request;
        }

        /// <inheritdoc />
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();

            var withContent = page.GetPageSize().GetWidth() - 70;
            PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
            Rectangle rootArea = new Rectangle(35, 0, withContent, 230);
            new Canvas(pdfCanvas, rootArea)
                .Add(this.GetTable(withContent));
        }

        private Table GetTable(float withContent)
        {
            Style styleCell = new Style()
                .SetFont(SuppliesWarehouseConstanst.FontFrutiger)
                .SetFontSize(8f)
                .SetFontColor(this.textColor)
                .SetBorder(Border.NO_BORDER)
                .SetMaxWidth(withContent)
                .SetBackgroundColor(this.signatureColor)
                .SetTextAlignment(TextAlignment.LEFT);

            var tableFooter = new Table(UnitValue.POINT).UseAllAvailableWidth();

            float[] cellWidth = { 45f, 10f, 45f };
            Table signatures = new Table(UnitValue.CreatePercentArray(cellWidth))
                .UseAllAvailableWidth();

            Table signatureApplicant = new Table(UnitValue.POINT)
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER);
            var cellSignatureApplicant = new Cell().Add(new Paragraph($"Firma solicitante"));
            signatureApplicant.AddCell(cellSignatureApplicant.AddStyle(styleCell).SetFont(SuppliesWarehouseConstanst.FontFrutigerBold));
            cellSignatureApplicant = new Cell().Add(this.GetSignature()?.SetAutoScale(true));
            signatureApplicant.AddCell(cellSignatureApplicant
                .AddStyle(styleCell)
                .SetMaxHeight(50)
                .SetMaxWidth(50)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetPaddingLeft(80));
            cellSignatureApplicant = new Cell().Add(new Paragraph(this.request.SigningUserName));
            signatureApplicant.AddCell(cellSignatureApplicant.AddStyle(styleCell).SetTextAlignment(TextAlignment.CENTER));
            cellSignatureApplicant = new Cell().Add(signatureApplicant);
            signatures.AddCell(cellSignatureApplicant.AddStyle(styleCell));

            var cellBlank = new Cell();
            signatures.AddCell(cellBlank.AddStyle(styleCell).SetBackgroundColor(ColorConstants.WHITE));

            Table warehouseSignature = new Table(UnitValue.POINT)
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER);
            var cellWarehouseSignature = new Cell().Add(new Paragraph($"Firma de almacén"));
            warehouseSignature.AddCell(cellWarehouseSignature.AddStyle(styleCell).SetFont(SuppliesWarehouseConstanst.FontFrutigerBold));
            cellWarehouseSignature = new Cell();
            warehouseSignature.AddCell(cellWarehouseSignature.AddStyle(styleCell).SetTextAlignment(TextAlignment.CENTER));
            cellWarehouseSignature = new Cell();
            warehouseSignature.AddCell(cellWarehouseSignature.AddStyle(styleCell).SetTextAlignment(TextAlignment.CENTER));
            cellWarehouseSignature = new Cell().Add(warehouseSignature);
            signatures.AddCell(cellWarehouseSignature.AddStyle(styleCell));

            var cellSignatures = new Cell().Add(signatures);
            tableFooter.AddCell(cellSignatures
                .AddStyle(styleCell)
                .SetBackgroundColor(ColorConstants.WHITE));

            var margin = new Cell().Add(signatures);
            tableFooter.AddCell(margin
                .AddStyle(styleCell)
                .SetBackgroundColor(ColorConstants.WHITE)
                .SetHeight(10f));

            var cellObservations = new Cell().Add(new Paragraph("Observaciones"));
            tableFooter.AddCell(cellObservations.AddStyle(styleCell).SetFont(SuppliesWarehouseConstanst.FontFrutigerBold));
            cellObservations = new Cell().Add(new Paragraph(this.request.Observations));
            tableFooter.AddCell(cellObservations
                .AddStyle(styleCell)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetPadding(5f));

            return tableFooter;
        }

        /// <summary>
        /// Add document signature.
        /// </summary>
        private Image GetSignature()
        {
            if (string.IsNullOrEmpty(this.request.Signature))
            {
                return new Image(ImageDataFactory.Create(1, 1, 3, 8, new byte[] { 255, 255, 255 }, null));
            }

            byte[] imageBytes = Convert.FromBase64String(this.request.Signature);
            return new Image(ImageDataFactory.Create(imageBytes));
        }
    }
}
