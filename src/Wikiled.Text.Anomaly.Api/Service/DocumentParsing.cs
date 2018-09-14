using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public class DocumentParsing : IDocumentParsing
    {
        private readonly IApiClient client;

        public DocumentParsing(IApiClientFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            client = factory.GetClient();
        }

        public async Task<Document[]> Extract(TextRequest request, CancellationToken token)
        {
            var result = await client.PostRequest<TextRequest, RawResponse<Document[]>>("api/parsing/processtext", request, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to retrieve:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }

        public async Task<Document[]> Extract(FileRequest request, CancellationToken token)
        {
            var result = await client.PostRequest<FileRequest, RawResponse<Document[]>>("api/parsing/processfile", request, token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to retrieve:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }
    }
}
