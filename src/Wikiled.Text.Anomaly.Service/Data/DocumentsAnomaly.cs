using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Service.Data
{
    public class DocumentsAnomaly
    {
        [FromRoute]
        [Required]
        public string Name { get; set; }

        [FromBody]
        [Required]
        public Document[] Documents { get; set; }
    }
}
