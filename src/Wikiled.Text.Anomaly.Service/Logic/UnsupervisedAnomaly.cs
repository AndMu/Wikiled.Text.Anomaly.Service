using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public class UnsupervisedAnomaly : IAnomalyDetection
    {
        private readonly ILogger<UnsupervisedAnomaly> logger;

        private readonly IAnomalyFactory anomalyFactory;

        public UnsupervisedAnomaly(ILoggerFactory logger, IAnomalyFactory anomalyFactory)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            
            this.anomalyFactory = anomalyFactory ?? throw new ArgumentNullException(nameof(anomalyFactory));
            this.logger = logger.CreateLogger<UnsupervisedAnomaly>();
        }

        public async Task<Document> RemoveAnomaly(RequestHeader requestHeader, Document[] documents)
        {
            logger.LogDebug("Performing anomaly detection");
            var anomaly = anomalyFactory.CreateSimple(new DocumentBlock(documents));
            var anomalyResult = await Task.Run(() => anomaly.Detect(requestHeader.AnomalyFilters)).ConfigureAwait(false);
            return anomalyResult.Document;
        }
    }
}
