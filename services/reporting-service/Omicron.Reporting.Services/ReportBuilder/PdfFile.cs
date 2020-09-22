// <summary>
// <copyright file="PdfFile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services.ReportBuilder
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Omicron.Reporting.Resources.Extensions;
    using Spire.Doc;

    /// <summary>
    /// PDF file utils.
    /// </summary>
    public static class PdfFile
    {
        /// <summary>
        /// Create a pdf stream from merge docx streams.
        /// </summary>
        /// <param name="docxStreams">Docx steams.</param>
        /// <returns>Pdf stream.</returns>
        public static MemoryStream CreatePdfStreamFromMergeDocxStreams(List<MemoryStream> docxStreams)
        {
            var pdfStreams = new List<MemoryStream>();

            foreach (var docxSubList in docxStreams.Split(3))
            {
                var document = new Document(docxSubList[0], FileFormat.Docx2013);

                docxSubList.Skip(1).ToList().ForEach(x => document.InsertTextFromStream(x, FileFormat.Docx2013));

                var pdfStream = new MemoryStream();
                document.SaveToStream(pdfStream, FileFormat.PDF);
                pdfStreams.Add(pdfStream);
            }

            return MergePdfStreams(pdfStreams.ToArray());
        }

        /// <summary>
        /// Merge pdf streams.
        /// </summary>
        /// <param name="pdfStreams">Pdf steams to merge.</param>
        /// <returns>Pdf stream.</returns>
        private static MemoryStream MergePdfStreams(params MemoryStream[] pdfStreams)
        {
            using var memoryStream = new MemoryStream();
            using (var document = new iTextSharp.text.Document())
            {
                using var smartCopy = new iTextSharp.text.pdf.PdfSmartCopy(document, memoryStream);
                smartCopy.CloseStream = false;
                document.Open();

                foreach (var pdfStream in pdfStreams)
                {
                    pdfStream.Position = 0;
                    using var reader = new iTextSharp.text.pdf.PdfReader(pdfStream);
                    smartCopy.AddDocument(reader);
                }

                document.Close();
            }

            return new MemoryStream(memoryStream.ToArray());
        }
    }
}
