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
    public class ParserController : BaseController
    {
        private readonly IAnomalyDetectionLogic anomalyDetection;

        private readonly IDocumentParser documentParser;

        public ParserController(ILoggerFactory loggerFactory,
            IAnomalyDetectionLogic anomalyDetection,
            IDocumentParser documentParser)
            : base(loggerFactory)
        {
            this.anomalyDetection = anomalyDetection ?? throw new ArgumentNullException(nameof(anomalyDetection));
            this.documentParser = documentParser;
        }

        [Route("processfile")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<AnomalyResult> Process([FromBody] FileAnomalyRequest request)
        {
            if (request.Data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Data));
            }

            var parsingResult = await documentParser.Parse(request.Name, request.Data, CancellationToken.None).ConfigureAwait(false);
            //parsingResult.Text.Pages.Select().SelectMany(item => item.Blocks).Select(item => item.Text)
            //request.Text = parsingResult.Text;
            var result = await anomalyDetection.Parse(request.Header, parsingResult.Document).ConfigureAwait(false);
            //var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            //return new AnomalyResult { Text = result.Text, Sentiment = rating.RawRating };
            throw new NotImplementedException();
        }
    }
}