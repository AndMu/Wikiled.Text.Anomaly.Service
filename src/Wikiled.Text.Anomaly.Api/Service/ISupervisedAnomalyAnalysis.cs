using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface ISupervisedAnomalyAnalysis
    {
        Task Add(TrainingData trainingData, CancellationToken token);

        Task Train(string name, CancellationToken token);

        Task<AnomalyResult> RemoveAnomaly(string name, TextAnomalyRequest requestHeader, CancellationToken token);

        Task<AnomalyResult> RemoveAnomaly(string name, FileAnomalyRequest request, CancellationToken token);
    }
}
