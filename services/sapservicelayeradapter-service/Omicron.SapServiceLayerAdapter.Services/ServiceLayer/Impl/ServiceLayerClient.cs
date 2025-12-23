// <summary>
// <copyright file="ServiceLayerClient.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.ServiceLayer.Impl
{
    /// <summary>
    /// Class representing a generic service layer client.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class ServiceLayerClient : IServiceLayerClient
    {
        private const string BatchBoundary = "batch_boundary";
        private const string ChangesetBoundary = "changeset_";

        // Expresiones Regulares
        private static readonly Regex BoundaryRegex = new Regex(@"boundary=(?<boundary>[^\r\n;]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ResponseHeaderRegex = new Regex(
            @"HTTP/1\.1\s+(?<StatusCode>\d+)\s+[^\r\n]*\r?\n(.*?)\r?\n(?<ContentBody>.*)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLayerClient"/> class with the specified HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for sending requests.</param>
        /// <param name="connection">The IServiceLayerAuth instance to use for get sessionId.</param>
        /// <param name="logger">The ILogger instance to logg information.</param>
        public ServiceLayerClient(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetAsync(string url)
        {
            ResultModel result;
            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PutAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PUT"), url);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.SendAsync(request))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger, requestBody);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PatchAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.SendAsync(request))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger, requestBody);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PostAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.PostAsync(url, content))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger, requestBody);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> DeleteAsync(string url)
        {
            ResultModel result;
            using (var response = await this.httpClient.DeleteAsync(url))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendBatchAsync(List<BatchOperationDto> changeset)
        {
            try
            {
                string changesetContent = this.BuildChangesetContent(changeset);
                string batchBody = this.BuildBatchContent(changesetContent);

                var batchBytes = Encoding.UTF8.GetBytes(batchBody);
                var content = new ByteArrayContent(batchBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("multipart/mixed");
                content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", BatchBoundary));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "$batch")
                {
                    Content = content,
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 0.9));
                using (var response = await this.httpClient.SendAsync(request))
                {
                    return await this.ParseAndHandleBatchResponse(response);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Error al procesar o enviar la petición Batch a Service Layer.");
                return new ResultModel
                {
                    Success = false,
                    UserError = ex.Message,
                    ExceptionMessage = ex.Message,
                    Code = (int)HttpStatusCode.InternalServerError,
                };
            }
        }

        private string BuildChangesetContent(List<BatchOperationDto> operations)
        {
            var sb = new StringBuilder();
            string currentChangesetBoundary = $"{ChangesetBoundary}{Guid.NewGuid():N}";

            sb.AppendLine($"--{currentChangesetBoundary}");

            for (int i = 0; i < operations.Count; i++)
            {
                var op = operations[i];
                string jsonBody = op.Body is string strBody ? strBody : JsonConvert.SerializeObject(op.Body);

                sb.AppendLine("Content-Type: application/http");
                sb.AppendLine("Content-Transfer-Encoding: binary");
                sb.AppendLine($"Content-ID: {op.ContentId}");
                sb.AppendLine();

                sb.AppendLine($"{op.Method.Method} {op.Url} HTTP/1.1");
                sb.AppendLine("Content-Type: application/json");
                sb.AppendLine($"Content-Length: {Encoding.UTF8.GetByteCount(jsonBody)}");
                sb.AppendLine();

                sb.AppendLine(jsonBody);
                if (i < operations.Count - 1)
                {
                    sb.AppendLine($"--{currentChangesetBoundary}");
                }
            }

            sb.AppendLine($"--{currentChangesetBoundary}--");
            return sb.ToString();
        }

        private string BuildBatchContent(string changesetContent)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--{BatchBoundary}");

            sb.AppendLine($"Content-Type: multipart/mixed; boundary={ChangesetBoundary}");
            sb.AppendLine();

            sb.Append(changesetContent);
            sb.AppendLine($"--{BatchBoundary}--");

            return sb.ToString();
        }

        private async Task<ResultModel> ParseAndHandleBatchResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return await ServiceUtils.GetServiceLayerResponse(response, this.logger);
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var results = this.ParseResponseContent(responseContent, response.Content.Headers.ContentType?.ToString());

            var firstError = results.FirstOrDefault(r => r.IsError);
            if (firstError != null)
            {
                this.logger.Error($"Petición Batch falló. Error en Content-ID: {firstError.ContentId}, Status: {firstError.StatusCode}");

                string errorMessage = "Se produjo un error en una o más operaciones del Batch.";
                if (!string.IsNullOrEmpty(firstError.ContentBody))
                {
                    errorMessage = this.TryToParseResponse(firstError.ContentBody);
                }

                return new ResultModel
                {
                    Success = false,
                    ExceptionMessage = errorMessage,
                    UserError = errorMessage,
                    Code = (int)firstError.StatusCode,
                };
            }

            return new ResultModel
            {
                Success = true,
                Code = (int)HttpStatusCode.OK,
            };
        }

        private string TryToParseResponse(string response)
        {
            try
            {
                var errorBody = JsonConvert.DeserializeObject<ServiceLayerErrorResponseDto>(response);
                return errorBody.Error.Message.Value;
            }
            catch (Exception)
            {
                return response;
            }
        }

        private List<ParsedBatchResultDto> ParseResponseContent(string responseContent, string contentType)
        {
            var results = new List<ParsedBatchResultDto>();

            var match = contentType != null ? BoundaryRegex.Match(contentType) : null;

            if (!match?.Success ?? true)
            {
                this.logger.Error($"No se pudo encontrar el boundary de respuesta en Content-Type: {contentType}");
                return results;
            }

            string boundary = "--" + match.Groups["boundary"].Value.Trim();

            var parts = responseContent
                .Trim()
                .Split(new[] { boundary }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !p.Trim().EndsWith("--"))
                .ToList();

            foreach (var part in parts)
            {
                var headerBodySplit = part.Trim().Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                if (headerBodySplit.Length < 2)
                {
                    continue;
                }

                string httpContent = headerBodySplit[1].Trim();
                var statusMatch = ResponseHeaderRegex.Match(httpContent);

                if (statusMatch.Success)
                {
                    int statusCode = int.Parse(statusMatch.Groups["StatusCode"].Value);

                    int contentId = 0;
                    var idMatch = Regex.Match(headerBodySplit[0], @"Content-ID:\s*(?<ID>\d+)", RegexOptions.IgnoreCase);
                    if (idMatch.Success)
                    {
                        contentId = int.Parse(idMatch.Groups["ID"].Value);
                    }

                    string fullHttpBody = statusMatch.Groups["ContentBody"].Value;
                    int jsonStartIndex = fullHttpBody.IndexOf("\r\n\r\n");

                    string contentBody = string.Empty;

                    if (jsonStartIndex >= 0)
                    {
                        contentBody = fullHttpBody.Substring(jsonStartIndex + 4).Trim();
                    }
                    else if (!string.IsNullOrWhiteSpace(fullHttpBody))
                    {
                        contentBody = fullHttpBody.Trim();
                    }

                    if (string.IsNullOrWhiteSpace(contentBody) || contentBody == "{}")
                    {
                        contentBody = string.Empty;
                    }

                    bool isError = statusCode >= 400;
                    results.Add(new ParsedBatchResultDto
                    {
                        ContentId = contentId,
                        StatusCode = (HttpStatusCode)statusCode,
                        ContentBody = contentBody,
                        IsError = isError,
                    });
                }
            }

            return results;
        }
    }
}