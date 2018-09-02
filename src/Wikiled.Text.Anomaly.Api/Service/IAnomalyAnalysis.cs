using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface IAnomalyAnalysis
    {
        Task<AnomalyResult> RemoveAnomaly(TextAnomalyRequest requestHeader, CancellationToken token);

        Task<AnomalyResult> RemoveAnomaly(FileAnomalyRequest request, CancellationToken token);

        Task<ExtractionResult> Extract(FileData request, CancellationToken token);
    }
}