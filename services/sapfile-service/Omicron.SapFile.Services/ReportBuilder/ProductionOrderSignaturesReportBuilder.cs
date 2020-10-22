// <summary>
// <copyright file="RawMaterialRequestReportBuilder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapFile.Services.ReportBuilder
{
    using System.Drawing;
    using System.IO;
    using Omicron.SapFile.Entities.Models;
    using Omicron.SapFile.Services.Constants;
    using Omicron.SapFile.Services.Utils;
    using Spire.Doc;
    using Spire.Doc.Documents;
    using Spire.Doc.Fields;

    /// <summary>
    /// Production order signatures report builder.
    /// </summary>
    public class ProductionOrderSignaturesReportBuilder
    {
        private const string STYLEBLACKTEXT = "BlackText";
        private const string BASEDOCUMENT = @"ReportBuilder/Templates/BASE_PO_SIGNATURES.docx";
        private readonly string rootDir;
        private readonly FinalizaGeneratePdfModel order;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrderSignaturesReportBuilder"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public ProductionOrderSignaturesReportBuilder(FinalizaGeneratePdfModel order)
        {
            this.rootDir = ServiceUtils.GetBinDirectory();
            this.order = order;
        }

        /// <summary>
        /// Build the report.
        /// </summary>
        /// <param name="output">PDF output file path.</param>
        public void BuildReport(string output)
        {
            var pdfMemoryStream = this.BuildReport();

            var fileStream = new FileStream(output, FileMode.Create, FileAccess.Write);
            pdfMemoryStream.Position = 0;
            pdfMemoryStream.WriteTo(fileStream);
            fileStream.Close();
            pdfMemoryStream.Close();
        }

        /// <summary>
        /// Build the report.
        /// </summary>
        /// <returns>Memory stream of pdf report.</returns>
        public MemoryStream BuildReport()
        {
            var document = new Document();
            document.LoadFromFile(Path.Combine(this.rootDir, BASEDOCUMENT));

            this.RegisterStyles(document);

            foreach (Section section in document.Sections)
            {
                foreach (Table table in section.Tables)
                {
                    if (table.Rows.Count > 2)
                    {
                        this.AddSignatures(table);
                    } 
                    else
                    {
                        this.AddOrderReferences(table);
                    }
                }
            }

            var padStream = new MemoryStream();
            document.SaveToStream(padStream, FileFormat.PDF);
            document.Dispose();
            return padStream;
        }

        /// <summary>
        /// Add document signature.
        /// </summary>
        /// <param name="cell">Cell to set signature.</param>
        /// <param name="imageBytes">Image to set.</param>
        private void AddSignaturePicture(TableCell cell, byte[] imageBytes)
        {
            cell.Paragraphs[0].ChildObjects.Clear();

            if (imageBytes == null || imageBytes.Length.Equals(0))
            {
                return;
            }

            using (var ms = new MemoryStream(imageBytes))
            {
                var image = Image.FromStream(ms);
                var picture = cell.Paragraphs[0].AppendPicture(image);
                picture.Width = 140;
                picture.Height = 100;
                picture.HorizontalAlignment = ShapeHorizontalAlignment.Center;
            }

            cell.Paragraphs[0].Format.HorizontalAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Add request items.
        /// </summary>
        /// <param name="table">Table to add request items.</param>
        /// <param name="productsSublist">Items to set.</param>
        private void AddSignatures(Table table)
        {
            var dataRowSignatures = table.Rows[0];
            var dataRowName = table.Rows[1];

            var dataRowSecondSignatures = table.Rows[3];
            var dataRowSecondNames = table.Rows[4];
            var dataTypeSignature = table.Rows[5];

            // QFB signature
            AddSignaturePicture(dataRowSignatures.Cells[0], this.order.QfbSignature);

            // Technical signature
            AddSignaturePicture(dataRowSignatures.Cells[2], this.order.TechnicalSignature);

            // Designer signature
            AddSignaturePicture(dataRowSecondSignatures.Cells[0], this.order.DesignerSignature);

            // QFB name
            dataRowName.Cells[0].ChildObjects.Clear();
            dataRowName.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            
            Paragraph cellContent = dataRowName.Cells[0].AddParagraph();
            cellContent.Format.HorizontalAlignment = HorizontalAlignment.Center;
            cellContent.AppendText(this.order.QfbName);
            cellContent.ApplyStyle(STYLEBLACKTEXT);

            // Designer Name
            var designerName = string.IsNullOrEmpty(this.order.DesignerName) ? string.Empty : this.order.DesignerName;
            dataRowSecondNames.Cells[0].ChildObjects.Clear();
            dataRowSecondNames.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            var cellContentName = dataRowSecondNames.Cells[0].AddParagraph();
            cellContentName.Format.HorizontalAlignment = HorizontalAlignment.Center;
            cellContentName.AppendText(designerName);
            cellContentName.ApplyStyle(STYLEBLACKTEXT);
        }

        /// <summary>
        /// Add order references.
        /// </summary>
        /// <param name="table">Table to add request items.</param>
        /// <param name="productsSublist">Items to set.</param>
        private void AddOrderReferences(Table table)
        {
            var firstReference = table.Rows[0];
            var secondReference = table.Rows[1];

            firstReference.Cells[0].ChildObjects.Clear();
            secondReference.Cells[0].ChildObjects.Clear();

            // First reference
            Paragraph firstCellContent = firstReference.Cells[0].AddParagraph();
            firstCellContent.ApplyStyle(STYLEBLACKTEXT);
            firstCellContent.Format.HorizontalAlignment = HorizontalAlignment.Right;

            // Second reference
            Paragraph secondCellContent = secondReference.Cells[0].AddParagraph();
            secondCellContent.ApplyStyle(STYLEBLACKTEXT);
            secondCellContent.Format.HorizontalAlignment = HorizontalAlignment.Right;

            if (this.order.OrderId != 0)
            {
                firstCellContent.AppendText(string.Format(ServiceConstants.SalesOrderReferenceText, this.order.OrderId));
                secondCellContent.AppendText(string.Format(ServiceConstants.ProductionOrderReferenceText, this.order.FabOrderId));
            }
            else
            {
                firstCellContent.AppendText(string.Format(ServiceConstants.ProductionOrderReferenceText, this.order.FabOrderId));
            }
        }

        /// <summary>
        /// Register styles to Document.
        /// </summary>
        /// <param name="document">Document instance to set styles.</param>
        private void RegisterStyles(Document document)
        { 
            var blackColor = "#787878";
            var font = "Arial";

            var styleForBlackText = new ParagraphStyle(document);
            styleForBlackText.Name = STYLEBLACKTEXT;
            styleForBlackText.CharacterFormat.TextColor = ColorTranslator.FromHtml(blackColor);
            styleForBlackText.CharacterFormat.FontName = font;
            styleForBlackText.CharacterFormat.FontSize = 12;
            document.Styles.Add(styleForBlackText);
        }
    }
}
