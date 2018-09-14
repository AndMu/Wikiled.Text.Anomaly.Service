using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Api.Request;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public class DocumentExtractor : IDocumentExtractor
    {
        private readonly ILogger<DocumentExtractor> logger;

        private readonly ISentimentAnalysisFactory sentimentAnalysisFactory;

        public DocumentExtractor(ILoggerFactory logger, ISentimentAnalysisFactory sentimentAnalysisFactory)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.sentimentAnalysisFactory = sentimentAnalysisFactory ?? throw new ArgumentNullException(nameof(sentimentAnalysisFactory));
            this.logger = logger.CreateLogger<DocumentExtractor>();
        }

        public Task<Document[]> Extract(string domain, RawDocument rawDocument)
        {
            logger.LogDebug("Parsing");
            SingleRequestData[] requests = new SingleRequestData[rawDocument.Pages.Length];
            for (int i = 0; i < rawDocument.Pages.Length; i++)
            {
                string text = rawDocument.Pages[i].Blocks.Select(x => x.Text).AccumulateItems(" ");
                SingleRequestData request = new SingleRequestData
                {
                    Text = text,
                    Id = i.ToString()
                };

                requests[i] = request;
            }

            return GetSentiment(domain, requests);
        }

        public Task<Document[]> Extract(string domain, string text)
        {
            SingleRequestData request = new SingleRequestData();
            request.Text = text;
            request.Id = "1";
            return GetSentiment(domain, request);
        }

        private async Task<Document[]> GetSentiment(string domain, params SingleRequestData[] requests)
        {
            Document[] result = await sentimentAnalysisFactory.Create(domain).Measure(requests, CancellationToken.None).ToArray();
            result = result.OrderBy(item => int.Parse(item.Id)).ToArray();
            return result;
        }
    }
}
