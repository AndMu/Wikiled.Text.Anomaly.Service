using Wikiled.Sentiment.Api.Service;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface ISentimentAnalysisFactory
    {
        ISentimentAnalysis Create(string domain);
    }
}