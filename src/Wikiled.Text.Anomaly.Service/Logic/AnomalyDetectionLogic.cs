using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Analysis.Structure;
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

        public async Task<Document> Parse(string text)
        {
            logger.LogDebug("Parsing");
            var document = await sentimentAnalysisFactory.Create("null").Measure(text, CancellationToken.None).ConfigureAwait(false);
            logger.LogDebug("Performing anomaly detection");
            var anomaly = anomalyFactory.CreateSimple(document);
            var result = await Task.Run(() => anomaly.Detect()).ConfigureAwait(false);
            return result;
        }
    }
}
