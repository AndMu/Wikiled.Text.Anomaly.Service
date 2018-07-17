using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Helpers;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Service.Logic;

namespace Wikiled.Text.Anomaly.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class ParserController : Controller
    {
        private readonly ILogger<ParserController> logger;

        private readonly IIpResolve resolve;

        private readonly IAnomalyDetectionLogic anomalyDetection;

        public ParserController(ILogger<ParserController> logger, IIpResolve resolve, IAnomalyDetectionLogic anomalyDetection)
        {
            this.resolve = resolve ?? throw new ArgumentNullException(nameof(resolve));
            this.anomalyDetection = anomalyDetection ?? throw new ArgumentNullException(nameof(anomalyDetection));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("version")]
        [HttpGet]
        public string ServerVersion()
        {
            var version = $"Version: [{Assembly.GetExecutingAssembly().GetName().Version}]";
            logger.LogInformation("Version request: {0}", version);
            return version;
        }

        [Route("process")]
        public async Task<AnomalyResult> Process(AnomalyRequest request)
        {
            var result = await anomalyDetection.Parse(request.Text).ConfigureAwait(false);
            var rating = RatingData.Accumulate(result.Sentences.Select(item => item.CalculateSentiment()));
            return new AnomalyResult {Text = result.Text, Sentiment = rating.RawRating};
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("processfile")]
        public ActionResult<AnomalyResult> ProcessFile([ModelBinder(BinderType = typeof(JsonModelBinder))] AnomalyRequest request, IFormFile file)
        {
            logger.LogInformation("Processing File: {0}", resolve.GetRequestIp());
            //if (file.Length > 0)
            //{
            //    var fileNameData = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            //    string fileName = fileNameData.Value;
            //    string fullPath = GetFileName(fileName);
            //    using (var stream = new FileStream(fullPath, FileMode.Create))
            //    {
            //        file.CopyTo(stream);
            //    }

            //    var fileInfo = new FileInfo(fullPath);
            //    var parser = parserFactory.ConstructParsers(fileInfo);
            //    var result = parser.Parse();
            //    AnomalyResult parsingResult = new AnomalyResult();
            //    parsingResult.Text = result;
            //    parsingResult.FileLength = fileNameData.Length;
            //    parsingResult.Name = fileInfo.Name;
            //    return Ok(parsingResult);
            //}

            //return StatusCode(500, "Failed processing");
            throw new NotImplementedException();
        }

        //private string GetFileName(string name)
        //{
        //    var path = config.Save;
        //    if (!Path.IsPathRooted(path))
        //    {
        //        string webRootPath = hostingEnvironment.ContentRootPath;
        //        path = Path.Combine(webRootPath, path);
        //    }

        //    path = Path.Combine(path, DateTime.Today.ToString("MMddyyyy"));
        //    if (!Directory.Exists(path))
        //    {
        //        lock (syncRoot)
        //        {
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }
        //        }
        //    }

        //    return Path.Combine(path, name);
        //}
    }
}
