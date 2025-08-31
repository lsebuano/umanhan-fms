using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class GoogleMapsSettings
    {
        public string ApiKey { get; set; }
    }

    public class GoogleMapsSettingsWrapper
    {
        public GoogleMapsSettings GoogleMaps { get; set; }
    }
}
