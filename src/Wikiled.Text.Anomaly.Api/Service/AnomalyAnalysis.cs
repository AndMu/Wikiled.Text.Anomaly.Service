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

        public async Task<AnomalyResult> Measure(AnomalyRequest request, CancellationToken token)
        {
            var result = await client.PostRequest<AnomalyRequest, ServiceResult<AnomalyResult>>("api/parser/process", request, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to retrieve:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }
    }
}
