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
    using Omicron.Pedidos.Services.Azure;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;
    using SkiaSharp;
    using SkiaSharp.QrCode;

    /// <summary>
    /// Class to create the Qrs.
    /// </summary>
    public class QrService : IQrService
    {
        private const int DefaultHeightWidth = 500;

        private const int DefaultMargin = 15;

        private readonly IPedidosDao pedidosDao;

        private readonly IConfiguration configuration;

        private readonly IAzureService azureService;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QrService"/> class.
        /// </summary>
        /// <param name="pedidosDao">The pedidos dao.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="azureService">the sap adapter.</param>
        /// <param name="almacenService">The almacen service.</param>
        public QrService(IPedidosDao pedidosDao, IConfiguration configuration, IAzureService azureService, IAlmacenService almacenService)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.configuration = configuration.ThrowIfNull(nameof(configuration));
            this.azureService = azureService.ThrowIfNull(nameof(azureService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateMagistralQr(List<int> ordersId)
        {
            var parameters = await this.pedidosDao.GetParamsByFieldContainsQueryOnly(ServiceConstants.MagistralQr);
            var azureAccount = this.configuration[ServiceConstants.AzureAccountName];
            var azureKey = this.configuration[ServiceConstants.AzureAccountKey];
            var azureContainer = this.configuration[ServiceConstants.OrderQrContainer];

            var saleOrders = await this.GetOrders(ordersId);
            var listSavedQr = await this.pedidosDao.GetQrRoute(saleOrders.Select(x => x.Id).ToList());

            var savedQrUserOrders = listSavedQr.Select(c => c.UserOrderId).ToList();
            var savedQrRoutes = listSavedQr.Select(r => r.MagistralQrRoute).ToList();

            saleOrders.RemoveAll(x => savedQrUserOrders.Contains(x.Id));

            var dimensionsQr = this.GetMagistralParameters(parameters);
            var urls = await this.GetUrlQrMagistral(saleOrders, dimensionsQr, savedQrRoutes, azureAccount, azureKey, azureContainer);
            urls.AddRange(savedQrRoutes);
            urls = urls.Distinct().ToList();

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateSampleLabel(List<int> ordersId)
        {
            var azureAccount = this.configuration[ServiceConstants.AzureAccountName];
            var azureKey = this.configuration[ServiceConstants.AzureAccountKey];
            var azureqrContainer = this.configuration[ServiceConstants.DeliveryQrContainer];

            var listSavedQr = await this.pedidosDao.GetQrRemisionRouteBySaleOrder(ordersId);

            var savedQrSaleOrder = listSavedQr.Select(c => c.PedidoId).ToList();
            var savedQrRoutes = listSavedQr.Select(r => r.RemisionQrRoute).ToList();

            ordersId.RemoveAll(x => savedQrSaleOrder.Contains(x));

            if (!ordersId.Any())
            {
                return ServiceUtils.CreateResult(true, 200, null, savedQrRoutes, null, null);
            }

            var idsStrings = ordersId.Select(x => x.ToString()).ToList();

            var parameters = await this.pedidosDao.GetParamsByFieldContainsQueryOnly(ServiceConstants.DeliveryQr);
            var saleOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(idsStrings)).ToList();

            if (!saleOrders.Any())
            {
                var lineProducts = await this.GetOrdersFromAlmacenDict(ServiceConstants.AlmacenGetOrderBySaleOrder, ordersId);
                lineProducts.ForEach(y =>
                {
                    var newOrder = new UserOrderModel
                    {
                        DeliveryId = y.DeliveryId,
                        RemisionQr = y.RemisionQr,
                        Salesorderid = y.SaleOrderId.ToString(),
                    };

                    saleOrders.Add(newOrder);
                });
            }

            saleOrders = saleOrders.Where(x => !string.IsNullOrEmpty(x.RemisionQr)).DistinctBy(y => y.Salesorderid).ToList();
            var dimensionsQr = this.GetDeliveryParameters(parameters);

            var urls = await this.GetUrlMuestraLabel(saleOrders, dimensionsQr, savedQrRoutes, azureAccount, azureKey, azureqrContainer);
            urls.AddRange(savedQrRoutes);
            urls = urls.Distinct().ToList();

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateRemisionQr(List<int> ordersId)
        {
            var azureAccount = this.configuration[ServiceConstants.AzureAccountName];
            var azureKey = this.configuration[ServiceConstants.AzureAccountKey];
            var azureqrContainer = this.configuration[ServiceConstants.DeliveryQrContainer];

            var listSavedQr = await this.pedidosDao.GetQrRemisionRouteByDelivery(ordersId);

            var savedQrRemision = listSavedQr.Select(c => c.RemisionId).ToList();
            var savedQrRoutes = listSavedQr.Select(r => r.RemisionQrRoute).ToList();

            ordersId.RemoveAll(x => savedQrRemision.Contains(x));

            if (!ordersId.Any())
            {
                return ServiceUtils.CreateResult(true, 200, null, savedQrRoutes, null, null);
            }

            var parameters = await this.pedidosDao.GetParamsByFieldContainsQueryOnly(ServiceConstants.DeliveryQr);
            var saleOrders = (await this.pedidosDao.GetUserOrderByDeliveryId(ordersId)).ToList();

            if (ServiceShared.CalculateOr(!saleOrders.Any(), saleOrders.Select(x => x.DeliveryId).Distinct().Count() < ordersId.Count))
            {
                var dictParam = $"?{ServiceConstants.Delivery}={JsonConvert.SerializeObject(ordersId)}";
                var route = $"{ServiceConstants.AlmacenGetOrders}{dictParam}";
                var lineProducts = await this.GetOrdersFromAlmacenDict(route, null);
                lineProducts.ForEach(y =>
                {
                    var newOrder = new UserOrderModel
                    {
                        DeliveryId = y.DeliveryId,
                        RemisionQr = y.RemisionQr,
                        Salesorderid = y.SaleOrderId.ToString(),
                    };

                    saleOrders.Add(newOrder);
                });
            }

            saleOrders = saleOrders.Where(x => !string.IsNullOrEmpty(x.RemisionQr)).DistinctBy(y => y.DeliveryId).ToList();
            var dimensionsQr = this.GetDeliveryParameters(parameters);
            var urls = await this.GetUrlQrRemision(saleOrders, dimensionsQr, savedQrRoutes, azureAccount, azureKey, azureqrContainer);

            urls.AddRange(savedQrRoutes);
            urls = urls.Distinct().ToList();

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateInvoiceQr(List<int> invoiceIds)
        {
            var azureAccount = this.configuration[ServiceConstants.AzureAccountName];
            var azureKey = this.configuration[ServiceConstants.AzureAccountKey];
            var azureqrContainer = this.configuration[ServiceConstants.InvoiceQrContainer];

            var listSavedQr = await this.pedidosDao.GetQrFacturaRouteByInvoice(invoiceIds);

            var savedQrFactura = listSavedQr.Select(c => c.FacturaId).ToList();
            var savedQrRoutes = listSavedQr.Select(r => r.FacturaQrRoute).ToList();

            invoiceIds.RemoveAll(x => savedQrFactura.Contains(x));

            if (!invoiceIds.Any())
            {
                return ServiceUtils.CreateResult(true, 200, null, savedQrRoutes, null, null);
            }

            var parameters = await this.pedidosDao.GetParamsByFieldContainsQueryOnly(ServiceConstants.DeliveryQr);
            var saleOrders = await this.pedidosDao.GetUserOrdersByInvoiceId(invoiceIds);

            if (!saleOrders.Any())
            {
                var lineProducts = await this.GetOrdersFromAlmacenDict(ServiceConstants.AlmacenGetOrderByInvoice, invoiceIds);
                lineProducts.ForEach(y =>
                {
                    var newOrder = new UserOrderModel
                    {
                        Salesorderid = y.SaleOrderId.ToString(),
                        InvoiceQr = y.InvoiceQr,
                    };

                    saleOrders.Add(newOrder);
                });
            }

            saleOrders = saleOrders.DistinctBy(x => x.InvoiceId).ToList();
            var dimensionsQr = this.GetDeliveryParameters(parameters);
            var urls = await this.GetUrlQrFactura(saleOrders, dimensionsQr, savedQrRoutes, azureAccount, azureKey, azureqrContainer);
            urls.AddRange(savedQrRoutes);
            urls = urls.Distinct().ToList();

            return ServiceUtils.CreateResult(true, 200, null, urls, null, null);
        }

        /// <summary>
        /// Get the lines product by ids.
        /// </summary>
        /// <param name="datatoSend">the orders.</param>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetOrdersFromAlmacenDict(string route, object datatoSend)
        {
            if (datatoSend == null)
            {
                var response = await this.almacenService.GetAlmacenData(route);
                return JsonConvert.DeserializeObject<List<LineProductsModel>>(response.Response.ToString());
            }

            var responsePost = await this.almacenService.PostAlmacenData(route, datatoSend);
            return JsonConvert.DeserializeObject<List<LineProductsModel>>(responsePost.Response.ToString());
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

        private async Task<List<string>> GetUrlQrMagistral(List<UserOrderModel> saleOrders, QrDimensionsModel parameters, List<string> existingUrls, string azureAccount, string azureKey, string container)
        {
            var listUrls = new List<string>();
            var listToSave = new List<ProductionOrderQr>();
            saleOrders = saleOrders.Where(x => !string.IsNullOrEmpty(x.MagistralQr)).ToList();
            byte[] dataQrBuffer;
            foreach (var so in saleOrders)
            {
                var modelQr = JsonConvert.DeserializeObject<MagistralQrModel>(so.MagistralQr);
                modelQr.Quantity = Math.Round(modelQr.Quantity, 1);
                using var surface = this.CreateSKQrCode(parameters, JsonConvert.SerializeObject(new { modelQr.SaleOrder, modelQr.ProductionOrder, modelQr.Quantity, modelQr.NeedsCooling, modelQr.ItemCode, Dxp = modelQr.DocNumDxp }));
                var needsCooling = modelQr.NeedsCooling.Equals("Y");
                var dxpDocNum = string.IsNullOrEmpty(modelQr.DocNumDxp) ? $"P:{modelQr.SaleOrder}" : $"S:{ServiceUtils.GetSubstring(modelQr.DocNumDxp, 6)} P:{modelQr.SaleOrder}";
                var topText = string.Format(ServiceConstants.QrTopTextOrden, dxpDocNum, modelQr.ItemCode);
                var pathTosave = string.Format(ServiceConstants.BlobUrlTemplate, azureAccount, container, $"{so.Productionorderid}qr.png");
                dataQrBuffer = this.AddTextToSKQr(surface, needsCooling, ServiceConstants.QrBottomTextOrden, modelQr.ProductionOrder.ToString(), parameters, true, topText);
                await this.UploadQrToAzure(dataQrBuffer, azureAccount, azureKey, pathTosave);

                var modelToSave = new ProductionOrderQr
                {
                    Id = Guid.NewGuid().ToString("D"),
                    MagistralQrRoute = pathTosave,
                    UserOrderId = so.Id,
                };

                if (!existingUrls.Contains(modelToSave.MagistralQrRoute))
                {
                    listToSave.Add(modelToSave);
                }

                listUrls.Add(pathTosave);
            }

            await this.pedidosDao.InsertQrRoute(listToSave);
            return listUrls;
        }

        private async Task<List<string>> GetUrlQrRemision(
            List<UserOrderModel> saleOrders,
            QrDimensionsModel parameters,
            List<string> existingUrls,
            string azureAccount,
            string azureKey,
            string container)
        {
            var listUrls = new List<string>();
            var listToSave = new List<ProductionRemisionQrModel>();
            saleOrders = saleOrders.Where(x => !string.IsNullOrEmpty(x.RemisionQr)).ToList();
            byte[] dataQrBuffer;
            foreach (var so in saleOrders)
            {
                var modelQr = JsonConvert.DeserializeObject<RemisionQrModel>(so.RemisionQr);
                using var surface = this.CreateSKQrCode(parameters, JsonConvert.SerializeObject(modelQr));

                if (!string.IsNullOrEmpty(modelQr.Ship))
                {
                    modelQr.Ship = ServiceShared.CalculateTernary(modelQr.Ship == ServiceConstants.LocalShipAbr, ServiceConstants.LocalShip, ServiceConstants.ForeignShip);
                }

                var topText = string.Format(ServiceConstants.QrTopTextRemision, modelQr.Ship);
                var remisionType = ServiceShared.CalculateTernary(string.IsNullOrEmpty(modelQr.Omi) || modelQr.Omi == "N", ServiceConstants.RemisionType, ServiceConstants.RemisionOmiType);
                var pathTosave = string.Format(ServiceConstants.BlobUrlTemplate, azureAccount, container, $"{modelQr.RemisionId}qr.png");
                dataQrBuffer = this.AddTextToSKQr(surface, modelQr.NeedsCooling, $"{remisionType}{ServiceConstants.QrBottomTextRemision}", modelQr.RemisionId.ToString(), parameters, false, topText);
                await this.UploadQrToAzure(dataQrBuffer, azureAccount, azureKey, pathTosave);

                var modelToSave = new ProductionRemisionQrModel
                {
                    Id = Guid.NewGuid().ToString("D"),
                    PedidoId = int.Parse(so.Salesorderid),
                    RemisionId = modelQr.RemisionId,
                    RemisionQrRoute = pathTosave,
                };

                if (!existingUrls.Contains(modelToSave.RemisionQrRoute))
                {
                    listToSave.Add(modelToSave);
                }

                listUrls.Add(pathTosave);
            }

            await this.pedidosDao.InsertQrRouteRemision(listToSave);
            return listUrls;
        }

        private async Task<List<string>> GetUrlMuestraLabel(List<UserOrderModel> saleOrders, QrDimensionsModel parameters, List<string> existingUrls, string azureAccount, string azureKey, string container)
        {
            var listUrls = new List<string>();
            var listToSave = new List<ProductionRemisionQrModel>();
            saleOrders = saleOrders.Where(x => !string.IsNullOrEmpty(x.RemisionQr)).ToList();
            byte[] dataQrBuffer;
            foreach (var so in saleOrders)
            {
                var modelQr = JsonConvert.DeserializeObject<RemisionQrModel>(so.RemisionQr);
                using var surface = this.DrawFilledSkRectangle(parameters.QrWidth, parameters.QrHeight);

                var topText = $"{modelQr.Ship}:";
                parameters.QrRectx = parameters.LabelSaleOrderRectx;
                parameters.QrRecty = parameters.LabelSaleOrderRecty;
                parameters.QrBottomTextSize = parameters.LabelMuestraFontSize;
                parameters.QrRectxTop = parameters.LabelRectx;
                parameters.QrRectyTop = parameters.LabelRecty;
                var pathTosave = string.Format(ServiceConstants.BlobUrlTemplate, azureAccount, container, $"MU{modelQr.PedidoId}qr.png");
                dataQrBuffer = this.AddTextToSKQr(surface, false, modelQr.PedidoId.ToString(), string.Empty, parameters, false, topText);
                await this.UploadQrToAzure(dataQrBuffer, azureAccount, azureKey, pathTosave);

                var modelToSave = new ProductionRemisionQrModel
                {
                    Id = Guid.NewGuid().ToString("D"),
                    PedidoId = int.Parse(so.Salesorderid),
                    RemisionId = modelQr.RemisionId,
                    RemisionQrRoute = pathTosave,
                };

                if (!existingUrls.Contains(modelToSave.RemisionQrRoute))
                {
                    listToSave.Add(modelToSave);
                }

                listUrls.Add(pathTosave);
            }

            await this.pedidosDao.InsertQrRouteRemision(listToSave);
            return listUrls;
        }

        private async Task<List<string>> GetUrlQrFactura(List<UserOrderModel> saleOrders, QrDimensionsModel parameters, List<string> existingUrls, string azureAccount, string azureKey, string container)
        {
            var listUrls = new List<string>();
            var listToSave = new List<ProductionFacturaQrModel>();
            saleOrders = saleOrders.Where(x => !string.IsNullOrEmpty(x.InvoiceQr)).ToList();
            byte[] dataQrBuffer;
            foreach (var so in saleOrders)
            {
                var modelQr = JsonConvert.DeserializeObject<InvoiceQrModel>(so.InvoiceQr);
                using var surface = this.CreateSKQrCode(parameters, JsonConvert.SerializeObject(modelQr));
                var pathTosave = string.Format(ServiceConstants.BlobUrlTemplate, azureAccount, container, $"{modelQr.InvoiceId}qr.png");
                dataQrBuffer = this.AddTextToSKQr(surface, modelQr.NeedsCooling, ServiceConstants.QrBottomTextFactura, modelQr.InvoiceId.ToString(), parameters, false);
                await this.UploadQrToAzure(dataQrBuffer, azureAccount, azureKey, pathTosave);
                var modelToSave = new ProductionFacturaQrModel
                {
                    Id = Guid.NewGuid().ToString("D"),
                    FacturaId = modelQr.InvoiceId,
                    FacturaQrRoute = pathTosave,
                };

                if (!existingUrls.Contains(modelToSave.FacturaQrRoute))
                {
                    listToSave.Add(modelToSave);
                }

                listUrls.Add(pathTosave);
            }

            await this.pedidosDao.InsertQrRouteFactura(listToSave);
            return listUrls;
        }

        private async Task UploadQrToAzure(
            byte[] dataQrBuffer,
            string azureAccount,
            string azureKey,
            string pathTosave)
        {
            using var streamQr = new MemoryStream(dataQrBuffer);
            streamQr.Position = 0;
            await this.azureService.UploadElementToAzure(azureAccount, azureKey, pathTosave, streamQr, ServiceConstants.PngFileFormat);
        }

        /// <summary>
        /// Creates the Qr with the data.
        /// </summary>
        /// <param name="parameters">The parameters data.</param>
        /// <param name="textToConvert">The text to use.</param>
        /// <returns>the bitmap.</returns>
        private SKSurface CreateSKQrCode(QrDimensionsModel parameters, string textToConvert)
        {
            using var generator = new QRCodeGenerator();
            var level = ECCLevel.H;
            var qr = generator.CreateQrCode(textToConvert, level, true);
            var info = new SKImageInfo(parameters.QrWidth, parameters.QrHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White); // Clear the canvas to ensure a clean background.
            var rect = SKRect.Create(parameters.QrCodePositionX, parameters.QrCodePositionY, parameters.QrCodeSize, parameters.QrCodeSize);
            canvas.Render(qr, rect, SKColor.Parse(ServiceConstants.HexWhiteColor), SKColor.Parse(ServiceConstants.HexBlackColor));
            return surface;
        }

        /// <summary>
        /// Add custom text to QR.
        /// </summary>
        /// <param name="width">Width to the Rectangle.</param>
        /// <param name="heigth">Heigth to the Rectangle.</param>
        /// <returns>SKSurface.</returns>
        private SKSurface DrawFilledSkRectangle(int width, int heigth)
        {
            var info = new SKImageInfo(width, heigth, SKColorType.Rgba8888, SKAlphaType.Premul);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            using var paint = new SKPaint();
            paint.Style = SKPaintStyle.Fill;
            paint.IsAntialias = true;
            canvas.Clear(SKColors.White);

            // canvas.DrawRect(0, 0, width, heigth, paint);
            return surface;
        }

        /// <summary>
        /// Add custom text to QR.
        /// </summary>
        /// <param name="surface">The SK Surface.</param>
        /// <param name="needsCoolingFlag">Flag if it needs cooling.</param>
        /// <param name="bottomTextTemplate">Template for the bottom text.</param>
        /// <param name="identifierToPlace">Identifier to place in the QR.</param>
        /// <param name="parameters">QR dimension parameters.</param>
        /// <param name="needTopTextRotated">Flag for rotating top text.</param>
        /// <param name="topText">Top text to draw.</param>
        private byte[] AddTextToSKQr(
            SKSurface surface,
            bool needsCoolingFlag,
            string bottomTextTemplate,
            string identifierToPlace,
            QrDimensionsModel parameters,
            bool needTopTextRotated,
            string topText = null)
        {
            var canvas = surface.Canvas;

            // Build the bottom text with cooling info if applicable.
            var needsCooling = needsCoolingFlag ? ServiceConstants.NeedsCooling : string.Empty;
            var bottomText = string.Format(bottomTextTemplate, identifierToPlace, needsCooling);

            // Configure SKPaint for drawing text.
            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                TextSize = parameters.QrBottomTextSize + 5,
                Typeface = this.GetFontTypeface(parameters.QrTextIsBold),
            };

            // Draw the bottom text.
            this.DrawRectangleSKText(paint, ref canvas, parameters.QrRectx, parameters.QrRecty, parameters.QrRectWidth, parameters.QrRectHeight, bottomText, false, 0);

            // Draw the top text if provided.
            if (!string.IsNullOrEmpty(topText))
            {
                this.CreateQrTopSkText(paint, ref canvas, parameters, topText, needTopTextRotated);
            }

            // Snapshot the canvas and encode it as a PNG.
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 90);
            return data.ToArray();
        }

        /// <summary>
        /// Get the appropriate typeface for text rendering.
        /// </summary>
        /// <param name="isBold">Indicates if the font should be bold.</param>
        /// <returns>The SKTypeface to use.</returns>
        private SKTypeface GetFontTypeface(bool isBold)
        {
            try
            {
                return SKTypeface.FromFamilyName(
                    ServiceConstants.QrTextFontType,
                    isBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                    SKFontStyleWidth.Normal,
                    SKFontStyleSlant.Upright) ?? SKTypeface.Default;
            }
            catch
            {
                // Fallback to default font in case of errors.
                return SKTypeface.Default;
            }
        }

        /// <summary>
        /// Draw text inside a rectangle on the canvas.
        /// </summary>
        private void DrawRectangleSKText(SKPaint paint, ref SKCanvas canvas, int rectx, int recty, int width, int height, string text, bool needRotate, int angleRotate)
        {
            if (needRotate)
            {
                canvas.Save(); // Save the current state of the canvas.
                canvas.RotateDegrees(angleRotate);
            }

            this.DrawTextArea(ref canvas, paint, rectx, recty, width, height, text);

            if (needRotate)
            {
                canvas.Restore(); // Restore the canvas to its original state.
            }
        }

        /// <summary>
        /// Draws multiline text in a defined area.
        /// </summary>
        private void DrawTextArea(ref SKCanvas canvas, SKPaint paint, float x, float y, float maxWidth, float lineHeight, string text)
        {
            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines = lines.SelectMany(l => this.SplitLine(paint, maxWidth, l, spaceWidth)).ToArray();

            foreach (var line in lines)
            {
                canvas.DrawText(line, x, y, paint);
                y += lineHeight; // Move to the next line.
            }
        }

        private string[] SplitLine(SKPaint paint, float maxWidth, string text, float spaceWidth)
        {
            var result = new List<string>();

            var words = text.Split(new[] { "\n" }, StringSplitOptions.None);

            var line = new StringBuilder();
            float width = 0;
            foreach (var word in words)
            {
                var wordWidth = paint.MeasureText(word);
                var wordWithSpaceWidth = wordWidth + spaceWidth;
                var wordWithSpace = word + " ";

                if (width + wordWidth > maxWidth)
                {
                    result.Add(line.ToString());
                    line = new StringBuilder(wordWithSpace);
                    width = wordWithSpaceWidth;
                }
                else
                {
                    line.Append(wordWithSpace);
                    width += wordWithSpaceWidth;
                }
            }

            result.Add(line.ToString());

            return result.ToArray();
        }

        /// <summary>
        /// Draw the top text in QR.
        /// </summary>
        /// <param name="paint">Sk Paint.</param>
        /// <param name="canvas">SK Canvas.</param>
        /// <param name="parameters">Parameters.</param>
        /// <param name="topText">Top text to draw.</param>
        /// <param name="textRotated">Needs the text rotated.</param>
        private void CreateQrTopSkText(SKPaint paint, ref SKCanvas canvas, QrDimensionsModel parameters, string topText, bool textRotated)
        {
            this.DrawRectangleSKText(paint, ref canvas, parameters.QrRectxTop, parameters.QrRectyTop, parameters.QrRectWidth, parameters.QrRectHeight, topText, textRotated, parameters.QrTopTextRotationAngle);
        }

        private QrDimensionsModel GetMagistralParameters(List<ParametersModel> parameters)
        {
            var rectHeigthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectHeight));
            var rectWidthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectWidth));
            var rectyField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRecty));
            var rectxField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectx));
            var rectxTopField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectxTop));
            var rectyTopField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralRectyTop));
            var sizeTextField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralBottomTextSize));
            var heigthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.MagistralQrHeight));
            var widthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.MagistralQrWidth));
            var marginField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.MagistralQrMargin));
            var rotateAngleTop = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralAngleRotTop));
            var textIsBold = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralTextIsBold));
            var positionXQrCode = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralCodePositionX));
            var positionYQrCode = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralCodePositionY));
            var sizeQrCode = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrMagistralCodeSize));

            return new QrDimensionsModel
            {
                QrRectHeight = ServiceShared.GetValueFromParamterIntParse(rectHeigthField, 250),
                QrRectWidth = ServiceShared.GetValueFromParamterIntParse(rectWidthField, 100),
                QrRecty = ServiceShared.GetValueFromParamterIntParse(rectyField, DefaultHeightWidth - 25),
                QrRectx = ServiceShared.GetValueFromParamterIntParse(rectxField, DefaultHeightWidth / 2),
                QrRectxTop = ServiceShared.GetValueFromParamterIntParse(rectxTopField, 130),
                QrRectyTop = ServiceShared.GetValueFromParamterIntParse(rectyTopField, 25),
                QrBottomTextSize = ServiceShared.GetValueFromParamterIntParse(sizeTextField, 24),
                QrHeight = ServiceShared.GetValueFromParamterIntParse(heigthField, DefaultHeightWidth),
                QrWidth = ServiceShared.GetValueFromParamterIntParse(widthField, DefaultHeightWidth),
                QrMargin = ServiceShared.GetValueFromParamterIntParse(marginField, DefaultMargin),
                QrTopTextRotationAngle = ServiceShared.GetValueFromParamterIntParse(rotateAngleTop, -90),
                QrTextIsBold = ServiceShared.GetValueFromParamterBooleanParse(textIsBold, true),
                QrCodePositionX = ServiceShared.GetValueFromParamterIntParse(positionXQrCode, (int)((DefaultMargin / 2) - (DefaultMargin * ServiceConstants.QrSize / 2))),
                QrCodePositionY = ServiceShared.GetValueFromParamterIntParse(positionYQrCode, (int)((DefaultMargin / 2) - (DefaultMargin * ServiceConstants.QrSize / 2))),
                QrCodeSize = ServiceShared.GetValueFromParamterIntParse(sizeQrCode, (int)(DefaultHeightWidth * ServiceConstants.QrSize)),
            };
        }

        private QrDimensionsModel GetDeliveryParameters(List<ParametersModel> parameters)
        {
            var rectHeigthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryRectHeight));
            var rectWidthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryRectWidth));
            var rectyField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryRecty));
            var rectxField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryRectx));
            var rectxTopField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryRectxTop));
            var rectyTopField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryRectyTop));
            var rectxLabelField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryLabelRectx));
            var rectyLabelField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryLabelRecty));
            var rectxLabelSaleField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryLabelSaleRectx));
            var rectyLabelSaleField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryLabelSaleRecty));
            var rectyLabelSaleFontSizeField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryLabelSaleFontSize));
            var sizeTextField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryBottomTextSize));
            var heigthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryHeight));
            var widthField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryWidth));
            var marginField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryMargin));
            var textIsBoldField = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryTextIsBold));
            var positionXQrCode = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryCodePositionX));
            var positionYQrCode = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryCodePositionY));
            var sizeQrCode = parameters.FirstOrDefault(x => x.Field.Equals(ServiceConstants.QrDeliveryCodeSize));

            return new QrDimensionsModel
            {
                QrRectHeight = ServiceShared.GetValueFromParamterIntParse(rectHeigthField, 250),
                QrRectWidth = ServiceShared.GetValueFromParamterIntParse(rectWidthField, 100),
                QrRecty = ServiceShared.GetValueFromParamterIntParse(rectyField, DefaultHeightWidth - 25),
                QrRectx = ServiceShared.GetValueFromParamterIntParse(rectxField, DefaultHeightWidth / 2),
                QrRectxTop = ServiceShared.GetValueFromParamterIntParse(rectxTopField, 130),
                QrRectyTop = ServiceShared.GetValueFromParamterIntParse(rectyTopField, 25),
                QrBottomTextSize = ServiceShared.GetValueFromParamterIntParse(sizeTextField, 24),
                QrHeight = ServiceShared.GetValueFromParamterIntParse(heigthField, DefaultHeightWidth),
                QrWidth = ServiceShared.GetValueFromParamterIntParse(widthField, DefaultHeightWidth),
                QrMargin = ServiceShared.GetValueFromParamterIntParse(marginField, DefaultMargin),
                LabelRectx = ServiceShared.GetValueFromParamterIntParse(rectxLabelField, DefaultHeightWidth / 2),
                LabelRecty = ServiceShared.GetValueFromParamterIntParse(rectyLabelField, DefaultHeightWidth / 2),
                LabelMuestraFontSize = ServiceShared.GetValueFromParamterIntParse(rectyLabelSaleFontSizeField, 24),
                LabelSaleOrderRectx = ServiceShared.GetValueFromParamterIntParse(rectxLabelSaleField, 150),
                LabelSaleOrderRecty = ServiceShared.GetValueFromParamterIntParse(rectyLabelSaleField, 250),
                QrTextIsBold = ServiceShared.GetValueFromParamterBooleanParse(textIsBoldField, true),
                QrCodePositionX = ServiceShared.GetValueFromParamterIntParse(positionXQrCode, (int)((DefaultMargin / 2) - (DefaultMargin * ServiceConstants.QrSize / 2))),
                QrCodePositionY = ServiceShared.GetValueFromParamterIntParse(positionYQrCode, (int)((DefaultMargin / 2) - (DefaultMargin * ServiceConstants.QrSize / 2))),
                QrCodeSize = ServiceShared.GetValueFromParamterIntParse(sizeQrCode, (int)(DefaultHeightWidth * ServiceConstants.QrSize)),
            };
        }
    }
}
