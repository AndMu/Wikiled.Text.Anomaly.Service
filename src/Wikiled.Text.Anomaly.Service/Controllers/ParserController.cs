﻿using System;
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

        [Route("process")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<AnomalyResult> Process([FromBody] AnomalyRequest request)
        {
            if (request.Data?.Length > 0)
            {
                var parsingResult = await documentParser.Parse(request.Name, request.Data, CancellationToken.None).ConfigureAwait(false);
                request.Text = parsingResult.Text;
            }

            var result = await anomalyDetection.Parse(request).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return new AnomalyResult { Text = result.Text, Sentiment = rating.RawRating };
        }
    }
}