using System.ComponentModel.DataAnnotations;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class TextAnomalyRequest
    {
        [Required]
        public AnomalyRequestHeader Header { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
