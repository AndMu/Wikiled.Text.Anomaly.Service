using System.ComponentModel.DataAnnotations;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class FileData
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
