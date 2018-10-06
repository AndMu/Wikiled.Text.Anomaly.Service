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

        public Task Add(DocumentAnomalyData anomalyData, CancellationToken token)
        {
            return client.PostRequest<DocumentAnomalyData, RawResponse<string>>("api/supervised/add", anomalyData, token).ProcessResult();
        }

        public Task Train(string name, CancellationToken token)
        {
            return client.GetRequest<RawResponse<string>>($"api/supervised/train/{name}", token).ProcessResult();
        }

        public Task Reset(string name, CancellationToken token)
        {
            return client.GetRequest<RawResponse<string>>($"api/supervised/reset/{name}", token).ProcessResult();
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
