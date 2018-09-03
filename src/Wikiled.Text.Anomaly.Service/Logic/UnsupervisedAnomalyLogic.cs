using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Api.Request;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public class UnsupervisedAnomalyLogic : IAnomalyDetection
    {
        private readonly ILogger<UnsupervisedAnomalyLogic> logger;

        private readonly IAnomalyFactory anomalyFactory;

        private readonly ISentimentAnalysisFactory sentimentAnalysisFactory;

        public UnsupervisedAnomalyLogic(ILoggerFactory logger, IAnomalyFactory anomalyFactory, ISentimentAnalysisFactory sentimentAnalysisFactory)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            
            this.anomalyFactory = anomalyFactory ?? throw new ArgumentNullException(nameof(anomalyFactory));
            this.sentimentAnalysisFactory = sentimentAnalysisFactory ?? throw new ArgumentNullException(nameof(sentimentAnalysisFactory));
            this.logger = logger.CreateLogger<UnsupervisedAnomalyLogic>();
        }

        public async Task<Document> RemoveAnomaly(AnomalyRequestHeader requestHeader, RawDocument rawDocument)
        {
            logger.LogDebug("Parsing");
            SingleRequestData[] requests = new SingleRequestData[rawDocument.Pages.Length];
            for (int i = 0; i < rawDocument.Pages.Length; i++)
            {
                var text = rawDocument.Pages[i].Blocks.Select(x => x.Text).AccumulateItems(" ");
                var request = new SingleRequestData();
                request.Text = text;
                request.Id = i.ToString();
                requests[i] = request;
            }

            var result = await sentimentAnalysisFactory.Create(requestHeader.Domain).Measure(requests, CancellationToken.None).ToArray();
            result = result.OrderBy(item => int.Parse(item.Id)).ToArray();
            logger.LogDebug("Performing anomaly detection");
            var anomaly = anomalyFactory.CreateSimple(new DocumentBlock(result));
            var anomalyResult = await Task.Run(() => anomaly.Detect(requestHeader.Filters)).ConfigureAwait(false);
            return anomalyResult.Document;
        }

        public Task<Document> RemoveAnomaly(AnomalyRequestHeader requestHeader, string text)
        {
            throw new NotImplementedException();
        }
    }
}
