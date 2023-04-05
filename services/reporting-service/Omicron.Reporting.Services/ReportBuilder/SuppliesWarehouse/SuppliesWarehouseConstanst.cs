// <summary>
// <copyright file="SuppliesWarehouseConstanst.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services.ReportBuilder.SuppliesWarehouse
{
    using System.IO;
    using System.Reflection;
    using iText.Kernel.Colors;
    using iText.Kernel.Font;

    /// <summary>
    /// SuppliesWarehouseConstanst class.
    /// </summary>
    public static class SuppliesWarehouseConstanst
    {
        /// <summary>
        /// Gets BlueStrong.
        /// </summary>
        /// <value>
        /// Color BlueStrong.
        /// </value>
        public static Color BlueStrong => new DeviceRgb(59, 128, 188);

        /// <summary>
        /// Gets RowColor.
        /// </summary>
        /// <value>
        /// Color RowColor.
        /// </value>
        public static Color RowColor => new DeviceRgb(177, 209, 238);

        /// <summary>
        /// Gets TextColor.
        /// </summary>
        /// <value>
        /// Color TextColor.
        /// </value>
        public static Color TextColor => new DeviceRgb(37, 37, 37);

        /// <summary>
        /// Gets TextBlue.
        /// </summary>
        /// <value>
        /// Color TextBlue.
        /// </value>
        public static Color TextBlue => new DeviceRgb(0, 112, 183);

        /// <summary>
        /// Gets TextBlue.
        /// </summary>
        /// <value>
        /// Color TextBlue.
        /// </value>
        public static PdfFont FontFrutigerBold => PdfFontFactory.CreateFont(
            File.ReadAllBytes($"{ROOR_DIR_FONTS}Frutiger Next Bold.ttf"),
            PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

        /// <summary>
        /// Gets TextBlue.
        /// </summary>
        /// <value>
        /// Color TextBlue.
        /// </value>
        public static PdfFont FontFrutiger => PdfFontFactory.CreateFont(
            File.ReadAllBytes($"{ROOR_DIR_FONTS}Frutiger Next Medium.ttf"),
            PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

        private static string ROOR_DIR => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static string ROOR_DIR_FONTS => string.Concat(ROOR_DIR, "/ReportBuilder/SuppliesWarehouse/Resources/");
    }
}
