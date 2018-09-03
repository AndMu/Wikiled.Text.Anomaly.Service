using System.ComponentModel.DataAnnotations;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class FileAnomalyRequest
    {
        [Required]
        public AnomalyRequestHeader Header { get; set; }

        [Required]
        public FileData FileData { get; set; }
    }
}
