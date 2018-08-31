using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Api.Request;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Processing;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public class AnomalyDetectionLogic : IAnomalyDetectionLogic
    {
        private readonly ILogger<AnomalyDetectionLogic> logger;

        private readonly IAnomalyFactory anomalyFactory;

        private readonly ISentimentAnalysisFactory sentimentAnalysisFactory;

        public AnomalyDetectionLogic(ILoggerFactory logger, IAnomalyFactory anomalyFactory, ISentimentAnalysisFactory sentimentAnalysisFactory)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            
            this.anomalyFactory = anomalyFactory ?? throw new ArgumentNullException(nameof(anomalyFactory));
            this.sentimentAnalysisFactory = sentimentAnalysisFactory ?? throw new ArgumentNullException(nameof(sentimentAnalysisFactory));
            this.logger = logger.CreateLogger<AnomalyDetectionLogic>();
        }

        public async Task<Document> Parse(AnomalyRequestHeader requestHeader, RawDocument rawDocument)
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
            var textBlock = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText("docs.json", textBlock);
            throw new NotImplementedException();
            //logger.LogDebug("Performing anomaly detection");
            //var anomaly = anomalyFactory.CreateSimple(document);
            //var result = await Task.Run(() => anomaly.Detect(requestHeader.Filters)).ConfigureAwait(false);
            //return result;
        }
    }
}
