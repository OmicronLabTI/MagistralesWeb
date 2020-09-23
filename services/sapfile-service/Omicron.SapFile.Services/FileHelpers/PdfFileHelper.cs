// <summary>
// <copyright file="PdfFileHelper.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.FileHelpers
{
    using System.Collections.Generic;
    using System.IO;
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    /// <summary>
    /// Helper for pdf files
    /// </summary>
    public static class PdfFileHelper
    {
        /// <summary>
        /// Add page number to file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public static void AddPageNumber(string filePath)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(filePath);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    for (int pageIndex = 1; pageIndex <= reader.NumberOfPages; pageIndex++)
                    {
                        var content = stamper.GetOverContent(pageIndex);
                        var layer = new PdfLayer("paginationLayer", stamper.Writer);
                        content.BeginLayer(layer);
                        content.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10);
                        content.SetColorFill(BaseColor.BLACK);
                        content.BeginText();
                        content.ShowTextAligned(Element.ALIGN_RIGHT, $"{pageIndex} de {reader.NumberOfPages}", reader.GetPageSize(pageIndex).Width - 27f, 15f, 0);
                        content.EndText();
                        content.EndLayer();
                    }
                }

                File.WriteAllBytes(SetFilePostFix(filePath, "paged"), stream.ToArray());
            }

        }

        /// <summary>
        /// Rotate file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public static void RotateFile(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);

            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    for (int pageIndex = 1; pageIndex <= reader.NumberOfPages; pageIndex++)
                    {
                        PdfDictionary page = reader.GetPageN(pageIndex);
                        PdfNumber rotate = page.GetAsNumber(PdfName.ROTATE);
                        if (rotate == null)
                        {
                            page.Put(PdfName.ROTATE, new PdfNumber(90));
                        }
                        else
                        {
                            page.Put(PdfName.ROTATE, new PdfNumber((rotate.IntValue + 90) % 360));
                        }
                    }
                }

                bytes = stream.ToArray();
            }

            File.WriteAllBytes(SetFilePostFix(filePath, "rotate"), bytes);
        }

        /// <summary>
        /// Merge pdf files
        /// </summary>
        /// <param name="pdfFilePaths">Path of files to merge.</param>
        /// <param name="outFilePath">Output file path.</param>
        /// <returns></returns>
        public static void MergePdfFiles(List<string> pdfFilePaths, string outFilePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    using (var smartCopy = new PdfSmartCopy(document, memoryStream))
                    {
                        smartCopy.CloseStream = false;
                        document.Open();

                        foreach (var filePath in pdfFilePaths)
                        {
                            var buffer = File.ReadAllBytes(filePath);
                            var pdfStream = new MemoryStream(buffer);
                            pdfStream.Position = 0;
                            using (var reader = new PdfReader(pdfStream))
                            {
                                smartCopy.AddDocument(reader);
                            }
                        }
                    }

                    document.Close();
                }

                SavePdfStreamToFile(outFilePath, memoryStream);
            }
        }

        /// <summary>
        /// Save stream to file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="pdfMemoryStream">File stream.</param>
        private static void SavePdfStreamToFile(string filePath, MemoryStream pdfMemoryStream)
        {
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            pdfMemoryStream.Position = 0;
            pdfMemoryStream.WriteTo(fileStream);
            fileStream.Close();
            pdfMemoryStream.Close();
        }

        /// <summary>
        /// Set a file path postfix.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="postFix">The post fix.</param>
        /// <returns>Updated path.</returns>
        private static string SetFilePostFix(string filePath, string postFix)
        {
            var directory = Path.GetDirectoryName(filePath);
            var file = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);
            return $"{directory}/{file}_{postFix}{extension}";
        }

    }
}
