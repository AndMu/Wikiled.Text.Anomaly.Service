using System.ComponentModel.DataAnnotations;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class FileRequest
    {
        [Required]
        public RequestHeader Header { get; set; }

        [Required]
        public FileData FileData { get; set; }
    }
}
