// <summary>
// <copyright file="RequestSuppliesWarehouse.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services.ReportBuilder.SuppliesWarehouse
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using iText.Kernel.Colors;
    using iText.Kernel.Events;
    using iText.Kernel.Font;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Borders;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Constants;

    /// <summary>
    /// RequestSuppliesWarehouse class.
    /// </summary>
    public class RequestSuppliesWarehouse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSuppliesWarehouse"/> class.
        /// </summary>
        public RequestSuppliesWarehouse()
        {
        }

        /// <summary>
        /// Method to build stream Pdf.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="products">Supplies warehouse.</param>
        /// <param name="streamOut">Stream out.</param>
        public void BuildPdf(RawMaterialRequestModel request, List<RawMaterialRequestDetailModel> products, MemoryStream streamOut)
        {
            using MemoryStream ms = new MemoryStream();
            using PdfWriter pw = new PdfWriter(ms);
            using PdfDocument pdfDocument = new PdfDocument(pw);
            using Document doc = new Document(pdfDocument, PageSize.LETTER);

            doc.SetMargins(75, 35, 240, 35);

            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderSuppliesWarehouseEventHandler());
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterSuppliesWarehouseEventHandler(request));

            Style styleHeader = new Style()
                .SetBackgroundColor(SuppliesWarehouseConstanst.BlueStrong)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(8f)
                .SetFontColor(ColorConstants.WHITE)
                .SetBorderLeft(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetBorderRight(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER);

            Style styleCellBlue = new Style()
                .SetBackgroundColor(SuppliesWarehouseConstanst.RowColor)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetFontSize(8f)
                .SetFontColor(SuppliesWarehouseConstanst.TextColor)
                .SetBorderLeft(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetBorderRight(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER);

            Style styleCellWhite = new Style()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetFontSize(8f)
                .SetFontColor(SuppliesWarehouseConstanst.TextColor)
                .SetBorderLeft(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetBorderRight(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER);

            float[] tableTitleCellWidth = { 80f, 20f, };
            Table tableTitle = new Table(UnitValue.CreatePercentArray(tableTitleCellWidth))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetMargins(0, 0, 10f, 0);

            Cell cellTitle = new Cell().Add(new Paragraph("Orden o solicitud de insumos al almacén"));
            tableTitle.AddCell(cellTitle
                .SetFont(SuppliesWarehouseConstanst.FontFrutigerBold)
                .SetFontColor(SuppliesWarehouseConstanst.TextBlue)
                .SetFontSize(15f)
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.BOTTOM));

            cellTitle = new Cell().Add(new Paragraph("Fecha"));
            tableTitle.AddCell(cellTitle
                .SetFont(SuppliesWarehouseConstanst.FontFrutigerBold)
                .SetFontColor(SuppliesWarehouseConstanst.TextColor)
                .SetFontSize(8f)
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.BOTTOM));

            cellTitle = new Cell().Add(new Paragraph($"Nº de Solicitud de traslado: {request.RequestNumber}"));
            tableTitle.AddCell(cellTitle
                .SetFont(SuppliesWarehouseConstanst.FontFrutigerBold)
                .SetFontColor(SuppliesWarehouseConstanst.TextColor)
                .SetFontSize(10f)
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            cellTitle = new Cell().Add(new Paragraph(request.CreationDate ?? DateTime.Now.ToString(DateConstants.LargeFormat)));
            tableTitle.AddCell(cellTitle
                .SetFont(SuppliesWarehouseConstanst.FontFrutiger)
                .SetFontColor(SuppliesWarehouseConstanst.TextColor)
                .SetFontSize(8f)
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.TOP));

            doc.Add(tableTitle);

            float[] tableSuppliesCellsWidth = { 10f, 50f, 10f, 20f, 10f, };
            Table tableSupplies = new Table(UnitValue.CreatePercentArray(tableSuppliesCellsWidth))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(SuppliesWarehouseConstanst.BlueStrong, 1))
                .SetMargins(0, 0, 10f, 0);

            Cell cellSupplie = new Cell().Add(new Paragraph("Código"));
            tableSupplies.AddHeaderCell(cellSupplie.AddStyle(styleHeader));

            cellSupplie = new Cell().Add(new Paragraph("Descripción"));
            tableSupplies.AddHeaderCell(cellSupplie.AddStyle(styleHeader));

            cellSupplie = new Cell().Add(new Paragraph("Cantidad"));
            tableSupplies.AddHeaderCell(cellSupplie.AddStyle(styleHeader));

            cellSupplie = new Cell().Add(new Paragraph("Almacén destino"));
            tableSupplies.AddHeaderCell(cellSupplie.AddStyle(styleHeader));

            cellSupplie = new Cell().Add(new Paragraph("Unidad"));
            tableSupplies.AddHeaderCell(cellSupplie.AddStyle(styleHeader));

            int rowNumber = 0;
            products.ForEach(product =>
            {
                cellSupplie = new Cell().Add(new Paragraph(product.ProductId));
                tableSupplies.AddCell(cellSupplie.AddStyle(this.GetRowStyle(styleCellBlue, styleCellWhite, rowNumber)));

                cellSupplie = new Cell().Add(new Paragraph(product.Description));
                tableSupplies.AddCell(cellSupplie
                    .AddStyle(this.GetRowStyle(styleCellBlue, styleCellWhite, rowNumber))
                    .SetTextAlignment(TextAlignment.JUSTIFIED));

                cellSupplie = new Cell().Add(new Paragraph(product.RequestQuantity.ToString()));
                tableSupplies.AddCell(cellSupplie.AddStyle(this.GetRowStyle(styleCellBlue, styleCellWhite, rowNumber)));

                cellSupplie = new Cell().Add(new Paragraph(product.Warehouse ?? string.Empty));
                tableSupplies.AddCell(cellSupplie.AddStyle(this.GetRowStyle(styleCellBlue, styleCellWhite, rowNumber)));

                cellSupplie = new Cell().Add(new Paragraph(product.Unit ?? string.Empty));
                tableSupplies.AddCell(cellSupplie.AddStyle(this.GetRowStyle(styleCellBlue, styleCellWhite, rowNumber)));
                rowNumber++;
            });

            doc.Add(tableSupplies);
            doc.Close();

            byte[] bytes = ms.ToArray();
            streamOut.Write(bytes, 0, bytes.Length);
            streamOut.Position = 0;
        }

        private Style GetRowStyle(Style styleCellBlue, Style styleCellWhite, int rowNumber)
        {
            return rowNumber % 2 == 0 ? styleCellWhite : styleCellBlue;
        }
    }
}
