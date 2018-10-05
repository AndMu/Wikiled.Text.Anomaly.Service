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

        public async Task Add(DocumentAnomalyData anomalyData, CancellationToken token)
        {
            var result = await client.PostRequest<DocumentAnomalyData, RawResponse<string>>("api/supervised/add", anomalyData, token).ConfigureAwait(false);
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

        public Task Reset(string name, CancellationToken token)
        {
            var result = await client.GetRequest<RawResponse<string>>($"api/supervised/reset/{name}", token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to train model:" + result.HttpResponseMessage);
            }
        }

        public Task<DocumentAnomalyData> Resolve(string name, Document[] documents, CancellationToken token)
        {
            return client
                .PostRequest<Document[], RawResponse<DocumentAnomalyData>>(
                    $"api/supervised/test/documents/{name}",
                    documents,
                    token)
                .ProcessResult();
        }

        public Task<SentenceAnomalyData> Resolve(string name, Document document, CancellationToken token)
        {
            return client
                .PostRequest<Document, RawResponse<SentenceAnomalyData>>(
                    $"api/supervised/test/sentences/{name}",
                    document,
                    token)
                .ProcessResult();
        }
    }
}
