using System.ComponentModel.DataAnnotations;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class RequestHeader
    {
        public FilterTypes[] AnomalyFilters { get; set; }

        public string Domain { get; set; }

        public string Name { get; set; }
    }
}
