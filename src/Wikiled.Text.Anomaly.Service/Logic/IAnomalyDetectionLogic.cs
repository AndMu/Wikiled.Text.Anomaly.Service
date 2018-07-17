using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface IAnomalyDetectionLogic
    {
        Task<Document> Parse(string text);
    }
}