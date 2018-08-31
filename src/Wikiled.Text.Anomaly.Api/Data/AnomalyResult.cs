using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class AnomalyResult
    {
        public Document[] Pages { get; set; }

        public double? Sentiment { get; set; }
    }
}
