﻿using System;
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
    public class SupervisedController : BaseController
    {
        private readonly ISupervisedAnomaly anomalyDetection;

        private readonly IDocumentParser documentParser;

        public SupervisedController(
            ILoggerFactory loggerFactory,
            ISupervisedAnomaly anomalyDetection,
            IDocumentParser documentParser)
            : base(loggerFactory)
        {
            this.anomalyDetection = anomalyDetection ?? throw new ArgumentNullException(nameof(anomalyDetection));
            this.documentParser = documentParser;
        }

        public ActionResult Add(TrainingData trainingData)
        {
            anomalyDetection.Add(trainingData);
            return Ok();
        }
         
        public Task Train(string name)
        {
            return anomalyDetection.Train(name);
        }
    }
}