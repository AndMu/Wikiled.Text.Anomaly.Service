using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public class AnomalyAnalysis : IAnomalyAnalysis
    {
        private readonly IApiClient client;

        public AnomalyAnalysis(IApiClientFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            client = factory.GetClient();
        }

        public Task<AnomalyResult> RemoveAnomaly(TextRequest requestHeader, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<AnomalyResult> RemoveAnomaly(FileRequest request, CancellationToken token)
        {
            var result = await client.PostRequest<FileRequest, RawResponse<AnomalyResult>>("api/anomaly/processfile", request, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to retrieve:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }

        public Task<ExtractionResult> Extract(FileData request, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
