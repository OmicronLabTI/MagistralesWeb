// <summary>
// <copyright file="QrService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;
    using ZXing;

    /// <summary>
    /// Class to create the Qrs.
    /// </summary>
    public class QrService : IQrService
    {
        private const int DefaultHeightWidth = 350;

        private const int DefaultMargin = 5;

        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="QrService"/> class.
        /// </summary>
        /// <param name="pedidosDao">The pedidos dao.</param>
        public QrService(IPedidosDao pedidosDao)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
        }

        /// <summary>
        /// Creates the qr for magistral.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> CreateMagistralQr(List<int> ordersId)
        {
            var saleOrders = await this.GetOrders(ordersId);
            var parameters = await this.pedidosDao.GetParamsByFieldContains(ServiceConstants.MagistralQr);
            var urls = this.GetUrlQr(saleOrders, parameters);

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <summary>
        /// Gets the orders by the order id.
        /// </summary>
        /// <param name="ordersId">The orders id.</param>
        /// <returns>The data.</returns>
        private async Task<List<UserOrderModel>> GetOrders(List<int> ordersId)
        {
            var stringOrdersId = ordersId.Select(x => x.ToString()).ToList();
            return (await this.pedidosDao.GetUserOrderByProducionOrder(stringOrdersId)).ToList();
        }

        /// <summary>
        /// Creates the url and the image.
        /// </summary>
        /// <param name="saleOrders">the sale order.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        private List<string> GetUrlQr(List<UserOrderModel> saleOrders, List<ParametersModel> parameters)
        {
            var listUrls = new List<string>();
            saleOrders
                .Where(x => !string.IsNullOrEmpty(x.MagistralQr))
                .ToList()
                .ForEach(so =>
                {
                    var modelQr = JsonConvert.DeserializeObject<MagistralQrModel>(so.MagistralQr);
                    var bitmap = this.CreateQr(parameters, so.MagistralQr);
                    bitmap = this.AddTextToQr(bitmap, modelQr, parameters);

                    var foldername = Path.Combine("Resources", "Images");
                    var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername, $"{so.Productionorderid}.png");

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), foldername)))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), foldername));
                    }

                    bitmap.Save(pathTosave, ImageFormat.Png);
                    listUrls.Add(pathTosave);
                });

            return listUrls;
        }

        /// <summary>
        /// Creates the Qr with the data.
        /// </summary>
        /// <param name="parameters">The parameters data.</param>
        /// <param name="textToConvert">The text to use.</param>
        /// <returns>the bitmap.</returns>
        private Bitmap CreateQr(List<ParametersModel> parameters, string textToConvert)
        {
            var heigthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.MagistralQrHeight));
            var widthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.MagistralQrWidth));
            var marginField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.MagistralQrMargin));

            var heigth = heigthField != null ? int.Parse(heigthField.Value) : DefaultHeightWidth;
            var width = widthField != null ? int.Parse(widthField.Value) : DefaultHeightWidth;
            var margin = marginField != null ? int.Parse(marginField.Value) : DefaultMargin;

            var writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions()
                {
                    Height = heigth,
                    Width = width,
                    Margin = margin,
                },
            };

            return writer.Write(textToConvert);
        }

        /// <summary>
        /// Add Bottoms lines to the QR.
        /// </summary>
        /// <param name="qrsBitmap">The bit maap.</param>
        /// <param name="magistralModel">The magistral model.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private Bitmap AddTextToQr(Bitmap qrsBitmap, MagistralQrModel magistralModel, List<ParametersModel> parameters)
        {
            var heigthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectHeight));
            var widthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectWidth));
            var rectyField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRecty));
            var rectxField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectx));

            var rectx = rectxField != null ? int.Parse(rectxField.Value) : DefaultHeightWidth / 2;
            var recty = rectyField != null ? int.Parse(rectyField.Value) : DefaultHeightWidth - 25;
            var heigth = heigthField != null ? int.Parse(heigthField.Value) : 250;
            var width = widthField != null ? int.Parse(widthField.Value) : 100;

            RectangleF rectf = new RectangleF(rectx, recty, heigth, width);

            var needsCooling = magistralModel.NeedsCooling.Equals("Y") ? ServiceConstants.NeedsCooling : string.Empty;
            var bottomText = string.Format(ServiceConstants.QrBottomText, magistralModel.ProductionOrder, needsCooling);

            var graphic = Graphics.FromImage(qrsBitmap);
            graphic.SmoothingMode = SmoothingMode.AntiAlias;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.DrawString(bottomText, new Font("Tahoma", 10), Brushes.Black, rectf);
            graphic.Flush();
            return qrsBitmap;
        }
    }
}
