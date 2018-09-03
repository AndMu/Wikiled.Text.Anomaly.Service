using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface ISupervisedAnalysis
    {
        Task Add(TrainingData trainingData, CancellationToken token);

        Task Train(string name, CancellationToken token);
    }
}
