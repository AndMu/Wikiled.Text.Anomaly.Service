using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface ISupervisedAnalysis
    {
        Task Add(DocumentAnomalyData anomalyData, CancellationToken token);

        Task Train(string name, CancellationToken token);

        Task Reset(string name, CancellationToken token);

        Task<DocumentAnomalyData> Resolve(string name, Document[] documents, CancellationToken token);

        Task<SentenceAnomalyData> Resolve(string name, Document document, CancellationToken token);
    }
}
