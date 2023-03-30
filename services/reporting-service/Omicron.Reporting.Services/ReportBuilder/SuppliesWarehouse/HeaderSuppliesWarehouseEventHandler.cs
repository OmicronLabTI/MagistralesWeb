// <summary>
// <copyright file="HeaderSuppliesWarehouseEventHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services.ReportBuilder.SuppliesWarehouse
{
    using iText.Kernel.Colors;
    using iText.Kernel.Events;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas;
    using iText.Layout;
    using iText.Layout.Borders;
    using iText.Layout.Element;
    using iText.Layout.Properties;

    /// <summary>
    /// HeaderSuppliesWarehouseEventHandler class.
    /// </summary>
    public class HeaderSuppliesWarehouseEventHandler : IEventHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderSuppliesWarehouseEventHandler"/> class.
        /// </summary>
        public HeaderSuppliesWarehouseEventHandler()
        {
        }

        /// <inheritdoc />
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();

            var pageNumber = pdfDoc.GetPageNumber(page);
            var numberOfPages = pdfDoc.GetNumberOfPages();

            PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
            Rectangle rootArea = new Rectangle(0, page.GetPageSize().GetTop() - 100, page.GetPageSize().GetWidth(), 100f);
            new Canvas(pdfCanvas, rootArea).Add(this.GetTable(pageNumber, numberOfPages));
        }

        private Table GetTable(int pageNumber, int numberOfPages)
        {
            Style styleCell = new Style()
                .SetFontColor(ColorConstants.WHITE)
                .SetBorder(Border.NO_BORDER)
                .SetMargins(0, 0, 0, 0)
                .SetPaddings(0, 0, 0, 0)
                .SetCharacterSpacing(0)
                .SetSpacingRatio(0)
                .SetFont(SuppliesWarehouseConstanst.FontFrutiger);

            float[] cellWidth = { 80f, 20f };
            Table tableBase = new Table(UnitValue.CreatePercentArray(cellWidth))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetBackgroundColor(SuppliesWarehouseConstanst.BlueStrong)
                .SetHeight(65f);

            Table tableTitle = new Table(1)
                .UseAllAvailableWidth()
                .SetMargins(0, 0, 0, 35f)
                .SetBorder(Border.NO_BORDER);

            var cellTitle = new Cell().Add(new Paragraph("OmicronLab"));
            tableTitle.AddCell(cellTitle
                .AddStyle(styleCell)
                .SetFont(SuppliesWarehouseConstanst.FontFrutigerBold)
                .SetFontSize(20f)
                .SetCharacterSpacing(2f)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                .SetPaddingLeft(70));

            cellTitle = new Cell().Add(new Paragraph("MEDICINA PERSONALIZADA"));
            tableTitle.AddCell(cellTitle
                .AddStyle(styleCell)
                .SetFont(SuppliesWarehouseConstanst.FontFrutigerBold)
                .SetFontSize(3.7f)
                .SetCharacterSpacing(4f)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.TOP)
                .SetPaddingLeft(70));

            var cellBase = new Cell().Add(tableTitle);
            tableBase.AddCell(cellBase
                .AddStyle(styleCell)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            cellBase = new Cell().Add(new Paragraph($"{pageNumber} de {numberOfPages}"));
            tableBase.AddCell(cellBase
                .AddStyle(styleCell)
                .SetFontSize(10f)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            return tableBase;
        }
    }
}
