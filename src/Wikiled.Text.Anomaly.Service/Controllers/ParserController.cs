using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
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
        public async Task<AnomalyResult> Process(AnomalyRequest request)
        {
            return await ProcessText(request.Text).ConfigureAwait(false);
        }

        private async Task<AnomalyResult> ProcessText(string text)
        {
            var result = await anomalyDetection.Parse(text).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return new AnomalyResult { Text = result.Text, Sentiment = rating.RawRating };
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("processfile")]
        public async Task<ActionResult> ProcessFile([ModelBinder(BinderType = typeof(JsonModelBinder))]
            AnomalyRequest request,
            IFormFile file)
        {
            if (file.Length != 0)
            {
                return StatusCode(500, "Only single file supported");
            }

            var fileNameData = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileName = fileNameData.Value;
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms).ConfigureAwait(false);
                fileBytes = ms.ToArray();
            }

            var parsingResult = await documentParser.Parse(fileName, fileBytes).ConfigureAwait(false);
            var result = await ProcessText(parsingResult.Text).ConfigureAwait(false);
            return Ok(result);
        }
    }
}