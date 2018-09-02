using System.ComponentModel.DataAnnotations;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class TrainingData
    {
        [Required]
        public string Name { get; set; }

        public Document[] Positive { get; set; }

        public Document[] Negative { get; set; }
    }
}
