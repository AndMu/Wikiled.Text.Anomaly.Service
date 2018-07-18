using System.ComponentModel.DataAnnotations;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class AnomalyRequest
    {
        [Required]
        public FilterTypes[] Filters { get; set; }

        public string Domain { get; set; }

        public string Text { get; set; }

        public byte[] Data { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
