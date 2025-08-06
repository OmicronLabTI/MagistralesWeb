// <summary>
// <copyright file="KafkaConsumerProductionOrderPdfGeneration.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api.Consumers
{
    using System.Collections.Generic;
    using System.Threading;
    using Confluent.Kafka;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Facade.Pedidos;

    /// <summary>
    /// KafkaConsumerProductionOrderPdfGeneration.
    /// </summary>
    public class KafkaConsumerProductionOrderPdfGeneration : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly ConsumerConfig consumer;
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumerProductionOrderPdfGeneration"/> class.
        /// </summary>
        /// <param name="configuration">Interface Configuration.</param>
        /// <param name="logger">Logger Configuration.</param>
        /// <param name="serviceProvider">Interface Service Provider.</param>
        public KafkaConsumerProductionOrderPdfGeneration(IConfiguration configuration, ILogger logger, IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.consumer = ConsumerUtils.GetConsumerConfig(configuration, KafkaConstants.KafkaTopicNameProductionOrderPdfGeneration);
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// ExecuteAsync.
        /// </summary>
        /// <param name="stoppingToken">stoppingToken.</param>
        /// <returns>Nothing.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => this.StartConsumer(stoppingToken));
            return Task.CompletedTask;
        }

        private async Task StartConsumer(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            Guid kafkaProcessId;
            var logBase = KafkaConstants.LogBaseKafkaConsumerProductionOrderPdfGeneration;
            this.logger.Debug(KafkaConstants.BackGroundStarting, logBase);
            stoppingToken.Register(() => this.logger.Debug(KafkaConstants.BackGroundStopping, logBase));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000, stoppingToken);
                kafkaProcessId = Guid.NewGuid();
                try
                {
                    using var consumerCredit = new ConsumerBuilder<long, string>(this.consumer).Build();
                    var lista = new List<string> { this.configuration[string.Format(KafkaConstants.KafkaEhName, KafkaConstants.KafkaTopicNameProductionOrderPdfGeneration)] };
                    consumerCredit.Subscribe(lista);

                    stopwatch = Stopwatch.StartNew();
                    var consumerResult = consumerCredit.Consume();
                    if (!string.IsNullOrEmpty(consumerResult.Message.Value))
                    {
                        var productionOrderProcessing = JsonConvert.DeserializeObject<ProductionOrderProcessingStatusDto>(consumerResult.Message.Value);
                        this.logger.Information(KafkaConstants.MessageReceived, productionOrderProcessing.Id, kafkaProcessId, logBase, consumerResult.Message.Value);
                        this.logger.Information(KafkaConstants.CreateScope, productionOrderProcessing.Id, kafkaProcessId, logBase);
                        using var scope = this.serviceProvider.CreateScope();
                        var pedidoFacade = scope.ServiceProvider.GetRequiredService<IPedidoFacade>();
                        this.logger.Information(KafkaConstants.CommitResult, productionOrderProcessing.Id, kafkaProcessId, logBase);
                        consumerCredit.Commit(consumerResult);
                        this.logger.Information(KafkaConstants.CallFinalizeProductionOrdersOnSap, productionOrderProcessing.Id, kafkaProcessId, logBase);
                        await pedidoFacade.ProductionOrderPdfGenerationAsync(productionOrderProcessing);
                        this.logger.Information(KafkaConstants.ProcessCompletedSuccessfully, productionOrderProcessing.Id, kafkaProcessId, logBase);
                    }
                }
                catch (Exception ex)
                {
                    var error = string.Format(KafkaConstants.ProcessTerminatedWithError, kafkaProcessId, logBase, ex.Message, ex.StackTrace);
                    this.logger.Error(ex, error);
                }
                finally
                {
                    stopwatch.Stop();
                    this.logger.Information(KafkaConstants.FinallyProcessCompleted, kafkaProcessId, logBase, stopwatch.Elapsed.TotalMilliseconds);
                }
            }
        }
    }
}
