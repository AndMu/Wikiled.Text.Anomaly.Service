using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface ISupervisedAnomaly
    {
        void Add(TrainingData trainingData);

        Task Train(string name);

        Document[] Resolve(string name, Document[] documents);

        SentenceItem[] Resolve(string name, Document document);
    }
}
