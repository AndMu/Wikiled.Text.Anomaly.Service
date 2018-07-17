using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Api.Data
{
    public class AnomalyRequest
    {
        public FilterTypes[] Filters { get; set; }

        public string Domain { get; set; }

        public string Text { get; set; }
    }
}
