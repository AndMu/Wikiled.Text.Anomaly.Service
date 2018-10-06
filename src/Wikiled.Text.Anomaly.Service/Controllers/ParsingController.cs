using System;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Controllers;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Service.Logic;
using Wikiled.Text.Parser.Api.Service;

namespace Wikiled.Text.Anomaly.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class ParsingController : BaseController
    {
        private readonly IDocumentExtractor extractor;

        private readonly IDocumentParser documentParser;

        public ParsingController(ILoggerFactory loggerFactory, IDocumentExtractor extractor, IDocumentParser documentParser)
            : base(loggerFactory)
        {
            this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            this.documentParser = documentParser ?? throw new ArgumentNullException(nameof(documentParser));
        }

        [Route("processfile")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<ActionResult<Document[]>> Process([FromBody] FileRequest request)
        {
            if (request.FileData.Data.Length <= 0)
            {
                return StatusCode(500, "No file data");
            }

            var parsingResult = await documentParser.Parse(request.FileData.Name, request.FileData.Data, CancellationToken.None).ConfigureAwait(false);
            var documents = await extractor.Extract(request.Header.Domain, parsingResult.Document).ConfigureAwait(false);
            return Ok(documents);
        }

        [Route("processtext")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<ActionResult<Document[]>> Process([FromBody] TextRequest request)
        {
            if (request.Text.Length <= 0)
            {
                return StatusCode(500, "No text data");
            }

            var documents = await extractor.Extract(request.Header.Domain, request.Text).ConfigureAwait(false);
            return Ok(documents);
        }
    }
}
