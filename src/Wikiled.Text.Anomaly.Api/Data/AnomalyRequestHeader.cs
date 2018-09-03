using System.ComponentModel.DataAnnotations;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class AnomalyRequestHeader
    {
        [Required]
        public FilterTypes[] Filters { get; set; }

        public string Domain { get; set; }

        public string Name { get; set; }
    }
}
