using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Controllers;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Service.Logic;
using Wikiled.Text.Parser.Api.Service;

namespace Wikiled.Text.Anomaly.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class AnomalyController : BaseController
    {
        private readonly IAnomalyDetectionLogic anomalyDetection;

        private readonly IDocumentParser documentParser;

        public AnomalyController(
            ILoggerFactory loggerFactory,
            IAnomalyDetectionLogic anomalyDetection,
            IDocumentParser documentParser)
            : base(loggerFactory)
        {
            this.anomalyDetection = anomalyDetection ?? throw new ArgumentNullException(nameof(anomalyDetection));
            this.documentParser = documentParser;
        }

        [Route("processfile")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<ActionResult<AnomalyResult>> Process([FromBody] FileAnomalyRequest request)
        {
            if (request.Data.Length <= 0)
            {
                return StatusCode(500, "No file data");
            }

            var parsingResult = await documentParser.Parse(request.Name, request.Data, CancellationToken.None).ConfigureAwait(false);
            var result = await anomalyDetection.RemoveAnomaly(request.Header, parsingResult.Document).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return Ok(new AnomalyResult { Document = result, Sentiment = rating.RawRating });
        }

        [Route("processtext")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<ActionResult<AnomalyResult>> Process([FromBody] TextAnomalyRequest request)
        {
            if (request.Text.Length <= 0)
            {
                return StatusCode(500, "No text data");
            }

            var result = await anomalyDetection.RemoveAnomaly(request.Header, request.Text).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return Ok(new AnomalyResult { Document = result, Sentiment = rating.RawRating });
        }
    }
}