// <summary>
// <copyright file="RawMaterialRequestReportBuilder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services.ReportBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Resources.Extensions;
    using Omicron.Reporting.Services.Constants;
    using Spire.Doc;
    using Spire.Doc.Documents;

    /// <summary>
    /// Raw material request report builder.
    /// </summary>
    public class RawMaterialRequestReportBuilder
    {
        private const string STYLEWHITETEXT = "WhiteText";
        private const string STYLEGRAYTEXT = "GrayText";
        private const string STYLESMALLGRAYTEXT = "SmallGrayText";
        private const string BASEDOCUMENT = @"ReportBuilder/Templates/BASE_RM_REQUEST.docx";
        private const string ARIALFONT = @"ReportBuilder/Templates/ARIAL.TTF";
        private readonly string rootDir;
        private readonly string creationDate;
        private readonly RawMaterialRequestModel request;
        private readonly List<MemoryStream> docFiles;
        private int currentPageNumber;
        private int totalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialRequestReportBuilder"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public RawMaterialRequestReportBuilder(RawMaterialRequestModel request)
        {
            this.rootDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.request = request;
            this.docFiles = new List<MemoryStream>();
            this.creationDate = DateTime.Now.ToString(DateConstants.LargeFormatWithOutSeconds);
        }

        /// <summary>
        /// Build the report.
        /// </summary>
        /// <returns>Memory stream of pdf report.</returns>
        public MemoryStream BuildReport()
        {
            var productsSublists = this.request.OrderedProducts.Split(10);
            this.currentPageNumber = 1;
            this.totalPages = productsSublists.Count;

            productsSublists.ForEach(x =>
                {
                    this.docFiles.Add(this.CreateDocument(x));
                    this.currentPageNumber++;
                });

            return PdfFile.CreatePdfStreamFromMergeDocxStreams(this.docFiles);
        }

        /// <summary>
        /// Create docx document.
        /// </summary>
        /// <param name="productsSublist">Items in the document.</param>
        /// <returns>Memory stream of document.</returns>
        private MemoryStream CreateDocument(List<RawMaterialRequestDetailModel> productsSublist)
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
                        this.AddRequestItems(table, productsSublist);
                    }

                    if (table.ChildObjects.Count == 1)
                    {
                        this.AddSignaturePicture(table);
                    }
                }

                foreach (Paragraph paragraph in section.Paragraphs)
                {
                    this.MapParagrap(paragraph);
                }
            }

            var documentStream = new MemoryStream();
            document.SaveToStream(documentStream, FileFormat.Docx2013);
            document.Dispose();
            return documentStream;
        }

        /// <summary>
        /// Map paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void MapParagrap(Paragraph paragraph)
        {
            foreach (DocumentObject childObject in paragraph.ChildObjects)
            {
                this.MapDocumentObject(childObject);
            }
        }

        /// <summary>
        /// Map document object.
        /// </summary>
        /// <param name="documentObject">The document object.</param>
        private void MapDocumentObject(DocumentObject documentObject)
        {
            if (documentObject.DocumentObjectType != DocumentObjectType.TextBox)
            {
                return;
            }

            var param = documentObject.FirstChild as Paragraph;
            param.ApplyStyle(STYLEGRAYTEXT);

            if (param.Text.Equals("date"))
            {
                param.ChildObjects.Clear();
                param.AppendText(this.creationDate);
                param.Format.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else if (param.Text.Equals("obs"))
            {
                param.ChildObjects.Clear();
                param.AppendText(this.request.Observations);
            }
            else if (param.Text.Equals("pagination"))
            {
                param.ChildObjects.Clear();
                param.AppendText($"{this.currentPageNumber} de {this.totalPages}");
                param.Format.HorizontalAlignment = HorizontalAlignment.Right;
                param.ApplyStyle(STYLEWHITETEXT);
            }
            else if (param.Text.Equals("signinguser"))
            {
                param.ChildObjects.Clear();
                param.AppendText($"{this.request.SigningUserName}");
                param.Format.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// Add document signature.
        /// </summary>
        /// <param name="table">Table to set dignature.</param>
        private void AddSignaturePicture(Table table)
        {
            table.Rows[0].Cells[0].Paragraphs[0].ChildObjects.Clear();

            if (string.IsNullOrEmpty(this.request.Signature))
            {
                return;
            }

            byte[] imageBytes = Convert.FromBase64String(this.request.Signature);
            using var ms = new MemoryStream(imageBytes);

            var image = Image.FromStream(ms);
            var picture = table.Rows[0].Cells[0].Paragraphs[0].AppendPicture(image);
            picture.Width = 80;
            picture.Height = 40;
            picture.TextWrappingStyle = TextWrappingStyle.Behind;
            picture.TextWrappingType = TextWrappingType.Both;
            picture.BehindText = true;

            table.Rows[0].Cells[0].Paragraphs[0].Format.HorizontalAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Add request items.
        /// </summary>
        /// <param name="table">Table to add request items.</param>
        /// <param name="productsSublist">Items to set.</param>
        private void AddRequestItems(Table table, List<RawMaterialRequestDetailModel> productsSublist)
        {
            for (var rowIndex = 0; rowIndex < productsSublist.Count; rowIndex++)
            {
                var dataRow = table.Rows[rowIndex];
                var product = productsSublist[rowIndex];
                for (var cellIndex = 0; cellIndex < dataRow.Cells.Count; cellIndex++)
                {
                    dataRow.Cells[cellIndex].ChildObjects.Clear();
                    dataRow.Cells[cellIndex].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph cellContent = dataRow.Cells[cellIndex].AddParagraph();
                    cellContent.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    var style = STYLEGRAYTEXT;

                    if (cellIndex == 0)
                    {
                        cellContent.AppendText(product.ProductId);
                        style = (product.ProductId.Length > 25) ? STYLESMALLGRAYTEXT : style;
                    }

                    if (cellIndex == 1)
                    {
                        cellContent.AppendText(product.Description);
                        cellContent.Format.HorizontalAlignment = HorizontalAlignment.Left;
                    }

                    if (cellIndex == 2)
                    {
                        cellContent.AppendText($"{product.RequestQuantity}");
                    }

                    if (cellIndex == 3)
                    {
                        cellContent.AppendText(product.Unit);
                    }

                    cellContent.ApplyStyle(style);
                }
            }
        }

        /// <summary>
        /// Register styles to Document.
        /// </summary>
        /// <param name="document">Document instance to set styles.</param>
        private void RegisterStyles(Document document)
        {
            var grayColor = "#a9a9a9";
            var whiteColor = "#FFFFFF";
            var fontPrivate = "ArialPrivate";

            var styleForGrayText = new ParagraphStyle(document);
            styleForGrayText.Name = STYLEGRAYTEXT;
            styleForGrayText.CharacterFormat.TextColor = ColorTranslator.FromHtml(grayColor);
            styleForGrayText.CharacterFormat.FontName = fontPrivate;
            styleForGrayText.CharacterFormat.FontSize = 9;
            document.Styles.Add(styleForGrayText);

            var styleForWhiteText = new ParagraphStyle(document);
            styleForWhiteText.Name = STYLEWHITETEXT;
            styleForWhiteText.CharacterFormat.TextColor = ColorTranslator.FromHtml(whiteColor);
            styleForWhiteText.CharacterFormat.FontName = fontPrivate;
            styleForWhiteText.CharacterFormat.FontSize = 9;
            document.Styles.Add(styleForWhiteText);

            var styleForSmallGrayText = new ParagraphStyle(document);
            styleForSmallGrayText.Name = STYLESMALLGRAYTEXT;
            styleForSmallGrayText.CharacterFormat.TextColor = ColorTranslator.FromHtml(grayColor);
            styleForSmallGrayText.CharacterFormat.FontSize = 7;
            styleForSmallGrayText.CharacterFormat.FontName = fontPrivate;

            document.Styles.Add(styleForSmallGrayText);

            document.PrivateFontList.Add(new PrivateFontPath(fontPrivate, Path.Combine(this.rootDir, ARIALFONT)));
        }
    }
}
