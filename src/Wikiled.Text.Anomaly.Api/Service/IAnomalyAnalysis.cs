using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface IAnomalyAnalysis
    {
        Task<AnomalyResult> Measure(TextAnomalyRequest requestHeader, CancellationToken token);

        Task<AnomalyResult> Measure(FileAnomalyRequest request, CancellationToken token);
    }
}