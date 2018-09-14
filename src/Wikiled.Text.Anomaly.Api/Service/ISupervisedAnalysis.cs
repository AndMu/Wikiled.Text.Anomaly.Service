using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface ISupervisedAnalysis
    {
        Task Add(TrainingData trainingData, CancellationToken token);

        Task Train(string name, CancellationToken token);

        Task<Document[]> Resolve(string name, Document[] documents, CancellationToken token);

        Task<SentenceItem[]> Resolve(string name, Document document, CancellationToken token);
    }
}
