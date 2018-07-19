using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Net.Client;
using Wikiled.Sentiment.Api.Request;
using Wikiled.Sentiment.Api.Service;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public class DomainSentimentAnalysisFactory : ISentimentAnalysisFactory
    {
        private readonly ILogger<DomainSentimentAnalysisFactory> logger;

        private readonly IStreamApiClientFactory streamApiClientFactory;

        private readonly Dictionary<string, WorkRequest> requestsTable = new Dictionary<string, WorkRequest>(StringComparer.OrdinalIgnoreCase);

        public DomainSentimentAnalysisFactory(ILoggerFactory factory, IStreamApiClientFactory streamApiClientFactory)
        {
            this.streamApiClientFactory = streamApiClientFactory ?? throw new ArgumentNullException(nameof(streamApiClientFactory));
            requestsTable.Add("market", new WorkRequest { Domain = "Market", CleanText = false });
            logger = factory?.CreateLogger<DomainSentimentAnalysisFactory>() ??throw new ArgumentNullException(nameof(factory));
        }

        public ISentimentAnalysis Create(string domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            if (!requestsTable.TryGetValue(domain, out var definition))
            {
                logger.LogDebug("Domain [{0}] definition not found. Using standard", domain);
                definition = new WorkRequest();
                definition.Domain = domain;
            }
            else
            {
                logger.LogDebug("Using predefined definition for domain [{0}]", domain);
            }

            return new SentimentAnalysis(streamApiClientFactory, definition);
        }
    }
}
