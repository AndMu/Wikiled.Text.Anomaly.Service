using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface IAnomalyDetection
    {
        Task<Document> RemoveAnomaly(RequestHeader requestHeader, Document[] documents);
    }
}