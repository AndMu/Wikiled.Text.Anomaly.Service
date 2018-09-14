using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public class SupervisedAnalysis : ISupervisedAnalysis
    {
        private readonly IApiClient client;

        public SupervisedAnalysis(IApiClientFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            client = factory.GetClient();
        }

        public async Task Add(TrainingData trainingData, CancellationToken token)
        {
            var result = await client.PostRequest<TrainingData, RawResponse<string>>("api/supervised/add", trainingData, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to add training data:" + result.HttpResponseMessage);
            }
        }

        public async Task Train(string name, CancellationToken token)
        {
            var result = await client.GetRequest<RawResponse<string>>($"api/supervised/train/{name}", token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to train model:" + result.HttpResponseMessage);
            }
        }

        public async Task<Document[]> Resolve(string name, Document[] documents, CancellationToken token)
        {
            var result = await client.PostRequest<Document[], RawResponse<Document[]>>($"api/supervised/test/documents/{name}", documents, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to add training data:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }

        public async Task<SentenceItem[]> Resolve(string name, Document document, CancellationToken token)
        {
            var result = await client.PostRequest<Document, RawResponse<SentenceItem[]>>($"api/supervised/test/sentences/{name}", document, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to add training data:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }
    }
}
