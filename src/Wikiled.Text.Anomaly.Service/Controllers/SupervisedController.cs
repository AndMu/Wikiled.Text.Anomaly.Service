using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Controllers;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Service.Data;
using Wikiled.Text.Anomaly.Service.Logic;

namespace Wikiled.Text.Anomaly.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class SupervisedController : BaseController
    {
        private readonly ISupervisedAnomaly anomalyDetection;

        public SupervisedController(ILoggerFactory loggerFactory, ISupervisedAnomaly anomalyDetection)
            : base(loggerFactory)
        {
            this.anomalyDetection = anomalyDetection ?? throw new ArgumentNullException(nameof(anomalyDetection));
        }

        [HttpPost("add")]
        public ActionResult Add([FromBody] TrainingData trainingData)
        {
            anomalyDetection.Add(trainingData);
            return Ok("Added");
        }

        [HttpGet("train/{name}")]
        public async Task<ActionResult> Train(string name)
        {
            await anomalyDetection.Train(name).ConfigureAwait(false);
            return Ok("Trained");
        }

        [HttpPost("test/documents/{name}")]
        public ActionResult<Document[]> Resolve(DocumentsAnomaly request)
        {
            var result = anomalyDetection.Resolve(request.Name, request.Documents);
            return Ok(result);
        }

        [HttpPost("test/sentences/{name}")]
        public ActionResult<SentenceItem[]> Resolve(DocumentAnomaly request)
        {
            var result = anomalyDetection.Resolve(request.Name, request.Document);
            return Ok(result);
        }
    }
}