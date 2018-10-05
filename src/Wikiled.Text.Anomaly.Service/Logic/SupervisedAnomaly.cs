using System;
using System.Collections.Generic;
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

        public void Add(DocumentAnomalyData anomalyData)
        {
            if (anomalyData == null)
            {
                throw new ArgumentNullException(nameof(anomalyData));
            }

            IModelStorage model = modelStorageFactory.Construct(anomalyData.Name);
            if (anomalyData.Positive?.Length > 0)
            {
                model.Add(DataType.Positive, anomalyData.Positive.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray());
            }

            if (anomalyData.Negative?.Length > 0)
            {
                model.Add(DataType.Negative, anomalyData.Negative.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray());
            }

            modelStorageFactory.Save(anomalyData.Name, model);
        }

        public async Task Train(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            IModelStorage model = modelStorageFactory.Construct(name);
            await model.Train(CancellationToken.None).ConfigureAwait(false);
            modelStorageFactory.Save(name, model);
        }

        public void Reset(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            IModelStorage model = modelStorageFactory.Construct(name);
            model.Reset();
            modelStorageFactory.Save(name, model);
        }

        public DocumentAnomalyData Resolve(string name, Document[] documents)
        {
            IModelStorage model = modelStorageFactory.Construct(name);
            if (model.Current == null)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Unknown model");
            }

            bool[] result = model.Current.Predict(new DocumentBlock(documents).Pages);
            List<Document> positive = new List<Document>();
            List<Document> negative = new List<Document>();
            for (int i = 0; i < result.Length; i++)
            {
                List<Document> list = result[i] ? positive : negative;
                list.Add(documents[i]);
            }

            return new DocumentAnomalyData
            {
                Name = name,
                Negative = negative.ToArray(),
                Positive = positive.ToArray()
            };
        }

        public SentenceAnomalyData Resolve(string name, Document document)
        {
            IModelStorage model = modelStorageFactory.Construct(name);
            if (model.Current == null)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Unknown model");
            }

            bool[] result = model.Current.Predict(document.Sentences.Select(item => new ProcessingTextBlock(item)).ToArray());
            List<SentenceItem> positive = new List<SentenceItem>();
            List<SentenceItem> negative = new List<SentenceItem>();
            for (int i = 0; i < result.Length; i++)
            {
                List<SentenceItem> list = result[i] ? positive : negative;
                list.Add(document.Sentences[i]);
            }

            return new SentenceAnomalyData
            {
                Name = name,
                Negative = negative.ToArray(),
                Positive = positive.ToArray()
            };
        }
    }
}
