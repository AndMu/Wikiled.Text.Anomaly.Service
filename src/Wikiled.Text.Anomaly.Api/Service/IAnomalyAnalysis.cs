﻿using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Api.Data;

namespace Wikiled.Text.Anomaly.Api.Service
{
    public interface IAnomalyAnalysis
    {
        Task<AnomalyResult> Measure(AnomalyRequest request, CancellationToken token);
    }
}