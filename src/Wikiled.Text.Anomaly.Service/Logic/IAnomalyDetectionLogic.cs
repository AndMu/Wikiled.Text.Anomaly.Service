using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface IAnomalyDetectionLogic
    {
        Task<Document> RemoveAnomaly(AnomalyRequestHeader requestHeader, RawDocument rawDocument);

        Task<Document> RemoveAnomaly(AnomalyRequestHeader requestHeader, string text);
    }
}