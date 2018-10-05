using System.ComponentModel.DataAnnotations;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class SentenceAnomalyData
    {
        [Required]
        public string Name { get; set; }

        public SentenceItem[] Positive { get; set; }

        public SentenceItem[] Negative { get; set; }
    }
}
