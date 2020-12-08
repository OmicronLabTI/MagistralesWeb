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
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.AlmacenService;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
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

        private readonly IConfiguration configuration;

        private readonly ISapAdapter sapAdapter;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QrService"/> class.
        /// </summary>
        /// <param name="pedidosDao">The pedidos dao.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="almacenService">The almacen service.</param>
        public QrService(IPedidosDao pedidosDao, IConfiguration configuration, ISapAdapter sapAdapter, IAlmacenService almacenService)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.almacenService = almacenService ?? throw new ArgumentNullException(nameof(almacenService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateMagistralQr(List<int> ordersId)
        {
            var parameters = await this.pedidosDao.GetParamsByFieldContains(ServiceConstants.MagistralQr);
            var saleOrders = await this.GetOrders(ordersId);

            var listSavedQr = await this.pedidosDao.GetQrRoute(saleOrders.Select(x => x.Id).ToList());

            var savedQrUserOrders = listSavedQr.Select(c => c.UserOrderId).ToList();
            var savedQrRoutes = listSavedQr.Select(r => r.MagistralQrRoute).ToList();

            saleOrders.RemoveAll(x => savedQrUserOrders.Contains(x.Id));
            var urls = await this.GetUrlQrMagistral(saleOrders, parameters);
            urls.AddRange(savedQrRoutes);

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateRemisionQr(List<int> ordersId)
        {
            var listSavedQr = await this.pedidosDao.GetQrRemisionRouteBySaleOrder(ordersId);

            var savedQrRemision = listSavedQr.Select(c => c.PedidoId).ToList();
            var savedQrRoutes = listSavedQr.Select(r => r.RemisionQrRoute).ToList();

            ordersId.RemoveAll(x => savedQrRemision.Contains(x));

            if (!ordersId.Any())
            {
                return ServiceUtils.CreateResult(true, 200, null, savedQrRoutes, null, null);
            }

            var parameters = await this.pedidosDao.GetParamsByFieldContains(ServiceConstants.MagistralQr);
            var saleOrders = await this.GetOrdersBySaleOrder(ordersId);

            if (!saleOrders.Any())
            {
                var lineProducts = await this.GetOrdersFromAlmacenDict(ordersId);
                lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).ToList().ForEach(y =>
                {
                    var newOrder = new UserOrderModel
                    {
                        Salesorderid = y.SaleOrderId.ToString(),
                        RemisionQr = y.RemisionQr,
                    };

                    saleOrders.Add(newOrder);
                });
            }

            var sapDelivery = await this.GetSapDelivery(ordersId);
            var urls = await this.GetUrlQrRemision(saleOrders, parameters, sapDelivery);
            urls.AddRange(savedQrRoutes);

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <summary>
        /// Gets the order from sap.
        /// </summary>
        /// <param name="ordersId">the orders to look.</param>
        /// <returns>the data.</returns>
        private async Task<List<DeliveryDetailModel>> GetSapDelivery(List<int> ordersId)
        {
            var response = await this.sapAdapter.PostSapAdapter(ordersId, ServiceConstants.GetDelivery);
            return JsonConvert.DeserializeObject<List<DeliveryDetailModel>>(response.Response.ToString());
        }

        /// <summary>
        /// Get the lines product by ids.
        /// </summary>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetOrdersFromAlmacenDict(List<int> ordersId)
        {
            var listString = new StringBuilder();
            ordersId.ForEach(x => listString.Append($"{x},"));

            var listIds = listString.ToString().Remove(listString.Length - 1, 1);

            var dictParam = $"?{ServiceConstants.SaleOrderId}={JsonConvert.SerializeObject(ordersId)}";
            var response = await this.almacenService.GetSapAdapter($"{ServiceConstants.AlmacenGetOrders}{dictParam}");
            return JsonConvert.DeserializeObject<List<LineProductsModel>>(response.Response.ToString());
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
        /// Gets the orders by the order id.
        /// </summary>
        /// <param name="ordersId">The orders id.</param>
        /// <returns>The data.</returns>
        private async Task<List<UserOrderModel>> GetOrdersBySaleOrder(List<int> ordersId)
        {
            var stringOrdersId = ordersId.Select(x => x.ToString()).ToList();
            return (await this.pedidosDao.GetUserOrderBySaleOrder(stringOrdersId)).ToList();
        }

        /// <summary>
        /// Creates the url and the image.
        /// </summary>
        /// <param name="saleOrders">the sale order.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        private async Task<List<string>> GetUrlQrMagistral(List<UserOrderModel> saleOrders, List<ParametersModel> parameters)
        {
            var baseAddres = this.configuration["QrImagesBaseRoute"];
            var listUrls = new List<string>();
            var listToSave = new List<ProductionOrderQr>();
            saleOrders
                .Where(x => !string.IsNullOrEmpty(x.MagistralQr))
                .ToList()
                .ForEach(so =>
                {
                    var modelQr = JsonConvert.DeserializeObject<MagistralQrModel>(so.MagistralQr);
                    var bitmap = this.CreateQr(parameters, so.MagistralQr);

                    var needsCooling = modelQr.NeedsCooling.Equals("Y");
                    bitmap = this.AddTextToQr(bitmap, needsCooling, ServiceConstants.QrBottomTextOrden, modelQr.ProductionOrder.ToString(), parameters);

                    var foldername = Path.Combine("Resources", "Images");
                    var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername, $"{so.Productionorderid}.png");

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), foldername)))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), foldername));
                    }

                    bitmap.Save(pathTosave, ImageFormat.Png);
                    var currentAddres = $"{baseAddres}{so.Productionorderid}.png";

                    var modelToSave = new ProductionOrderQr
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        MagistralQrRoute = currentAddres,
                        UserOrderId = so.Id,
                    };

                    listToSave.Add(modelToSave);
                    listUrls.Add(currentAddres);
                });

            await this.pedidosDao.InsertQrRoute(listToSave);
            return listUrls;
        }

        /// <summary>
        /// Creates the url and the image.
        /// </summary>
        /// <param name="saleOrders">the sale order.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        private async Task<List<string>> GetUrlQrRemision(List<UserOrderModel> saleOrders, List<ParametersModel> parameters, List<DeliveryDetailModel> sapDeliveries)
        {
            var baseAddres = this.configuration["QrImagesBaseRoute"];
            var listUrls = new List<string>();
            var listToSave = new List<ProductionRemisionQrModel>();
            saleOrders
                .Where(x => !string.IsNullOrEmpty(x.RemisionQr))
                .ToList()
                .ForEach(so =>
                {
                    var modelQr = JsonConvert.DeserializeObject<RemisionQrModel>(so.RemisionQr);
                    var bitmap = this.CreateQr(parameters, so.RemisionQr);

                    var delivery = sapDeliveries.FirstOrDefault(y => y.BaseEntry.ToString().Equals(so.Salesorderid));
                    delivery = delivery == null ? new DeliveryDetailModel() : delivery;

                    var needsCooling = modelQr.NeedsCooling.Equals("Y");
                    bitmap = this.AddTextToQr(bitmap, needsCooling, ServiceConstants.QrBottomTextRemision, delivery.DeliveryId.ToString(), parameters);

                    var foldername = Path.Combine("Resources", "Images");
                    var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername, $"{delivery.DeliveryId}.png");

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), foldername)))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), foldername));
                    }

                    bitmap.Save(pathTosave, ImageFormat.Png);
                    var currentAddres = $"{baseAddres}{delivery.DeliveryId}.png";

                    var modelToSave = new ProductionRemisionQrModel
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        PedidoId = int.Parse(so.Salesorderid),
                        RemisionId = delivery.DeliveryId,
                        RemisionQrRoute = currentAddres,
                    };

                    listToSave.Add(modelToSave);
                    listUrls.Add(currentAddres);
                });

            await this.pedidosDao.InsertQrRouteRemision(listToSave);
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
        /// Add custom text to QR.
        /// </summary>
        /// <param name="qrsBitmap">the bitmap.</param>
        /// <param name="needsCoolingFlag">the flag if it needs cooling.</param>
        /// <param name="botomText">the botom text.</param>
        /// <param name="identifierToPlace">the id to place.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the bitmap to return.</returns>
        private Bitmap AddTextToQr(Bitmap qrsBitmap, bool needsCoolingFlag, string botomText, string identifierToPlace, List<ParametersModel> parameters)
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

            var needsCooling = needsCoolingFlag ? ServiceConstants.NeedsCooling : string.Empty;
            var bottomText = string.Format(botomText, identifierToPlace, needsCooling);

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
