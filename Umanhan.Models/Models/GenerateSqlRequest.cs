using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class GenerateSqlRequest
    {
        public string SchemaDescription { get; set; }
        public string UserPrompt { get; set; }
    }
}
