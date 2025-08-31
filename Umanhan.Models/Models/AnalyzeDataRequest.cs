using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class AnalyzeDataRequest
    {
        public string ConvoId { get; set; }
        public string Prompt { get; set; }
        public dynamic Data { get; set; }
    }
}
