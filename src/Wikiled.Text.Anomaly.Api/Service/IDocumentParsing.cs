using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface IDocumentParsing
    {
        Task<Document[]> Extract(FileRequest request, CancellationToken token);

        Task<Document[]> Extract(TextRequest request, CancellationToken token);
    }
}