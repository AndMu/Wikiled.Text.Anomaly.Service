using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Supervised;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public class SupervisedAnomaly : ISupervisedAnomaly
    {
        private readonly IModelStorageFactory modelStorageFactory;

        public SupervisedAnomaly(IModelStorageFactory modelStorageFactory)
        {
            this.modelStorageFactory = modelStorageFactory ?? throw new ArgumentNullException(nameof(modelStorageFactory));
        }

        public void Add(TrainingData trainingData)
        {
            if (trainingData == null)
            {
                throw new ArgumentNullException(nameof(trainingData));
            }

            IModelStorage model = modelStorageFactory.Construct(trainingData.Name);
            if (trainingData.Positive?.Length > 0)
            {
                model.Add(DataType.Positive, trainingData.Positive.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray());
            }

            if (trainingData.Negative?.Length > 0)
            {
                model.Add(DataType.Negative, trainingData.Negative.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray());
            }

            modelStorageFactory.Save(trainingData.Name, model);
        }

        public async Task Train(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            var model = modelStorageFactory.Construct(name);
            await model.Train(CancellationToken.None).ConfigureAwait(false);
            modelStorageFactory.Save(name, model);
        }

        public Document[] Resolve(string name, Document[] documents)
        {
            throw new NotImplementedException();
        }

        public SentenceItem[] Resolve(string name, Document document)
        {
            throw new NotImplementedException();
        }
    }
}
