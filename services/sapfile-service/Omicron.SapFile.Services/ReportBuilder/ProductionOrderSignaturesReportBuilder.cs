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
    using Omicron.SapFile.Services.Utils;
    using Spire.Doc;
    using Spire.Doc.Documents;

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
                    if (table.ChildObjects.Count > 1)
                    {
                        this.AddSignatures(table);
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

            // QFB signature
            AddSignaturePicture(dataRowSignatures.Cells[0], this.order.QfbSignature);

            // Technical signature
            AddSignaturePicture(dataRowSignatures.Cells[2], this.order.TechnicalSignature);

            // QFB name
            dataRowName.Cells[0].ChildObjects.Clear();
            dataRowName.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            
            Paragraph cellContent = dataRowName.Cells[0].AddParagraph();
            cellContent.Format.HorizontalAlignment = HorizontalAlignment.Center;
            cellContent.AppendText(this.order.QfbName);
            cellContent.ApplyStyle(STYLEBLACKTEXT);
        }

        /// <summary>
        /// Register styles to Document.
        /// </summary>
        /// <param name="document">Document instance to set styles.</param>
        private void RegisterStyles(Document document)
        { 
            var blackColor = "#000000";
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
