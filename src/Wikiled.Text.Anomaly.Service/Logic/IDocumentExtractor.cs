using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Anomaly.Service.Logic
{
    public interface IDocumentExtractor
    {
        Task<Document[]> Extract(string domain, RawDocument rawDocument);

        Task<Document[]> Extract(string domain, string text);
    }
}