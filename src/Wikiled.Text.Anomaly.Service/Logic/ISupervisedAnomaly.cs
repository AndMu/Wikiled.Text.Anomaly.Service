using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface ISupervisedAnomaly
    {
        void Add(DocumentAnomalyData anomalyData);

        Task Train(string name);

        void Reset(string name);

        DocumentAnomalyData Resolve(string name, Document[] documents);

        SentenceAnomalyData Resolve(string name, Document document);
    }
}
