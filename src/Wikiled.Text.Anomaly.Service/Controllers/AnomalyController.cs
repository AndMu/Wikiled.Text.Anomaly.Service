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
    public class AnomalyController : BaseController
    {
        private readonly IAnomalyDetection anomalyDetection;

        private readonly IDocumentParser documentParser;

        private readonly IDocumentExtractor extractor;

        public AnomalyController(
            ILoggerFactory loggerFactory,
            IAnomalyDetection anomalyDetection,
            IDocumentParser documentParser,
            IDocumentExtractor extractor)
            : base(loggerFactory)
        {
            this.anomalyDetection = anomalyDetection ?? throw new ArgumentNullException(nameof(anomalyDetection));
            this.documentParser = documentParser ?? throw new ArgumentNullException(nameof(documentParser));
            this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        }

        [Route("processfile")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<ActionResult<AnomalyResult>> Process([FromBody] FileRequest request)
        {
            if (request.FileData.Data.Length <= 0)
            {
                return StatusCode(500, "No file data");
            }

            var parsingResult = await documentParser.Parse(request.FileData.Name, request.FileData.Data, CancellationToken.None).ConfigureAwait(false);
            var documents = await extractor.Extract(request.Header.Domain, parsingResult.Document).ConfigureAwait(false);
            var result = await anomalyDetection.RemoveAnomaly(request.Header, documents).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return Ok(new AnomalyResult { Document = result, Sentiment = rating.RawRating });
        }

        [Route("processtext")]
        [RequestSizeLimit(1024 * 1024 * 100)]
        public async Task<ActionResult<AnomalyResult>> Process([FromBody] TextRequest request)
        {
            if (request.Text.Length <= 0)
            {
                return StatusCode(500, "No text data");
            }

            var documents = await extractor.Extract(request.Header.Domain, request.Text).ConfigureAwait(false);
            var result = await anomalyDetection.RemoveAnomaly(request.Header, documents).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return Ok(new AnomalyResult { Document = result, Sentiment = rating.RawRating });
        }
    }
}