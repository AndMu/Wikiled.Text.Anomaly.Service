using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface ISupervisedAnomaly : IAnomalyDetection
    {
        void Add(TrainingData trainingData);

        Task Train(string name);
    }
}
