using System.ComponentModel.DataAnnotations;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class TextRequest
    {
        [Required]
        public RequestHeader Header { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
