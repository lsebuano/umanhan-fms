using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class FarmData
    {
        public int FarmId { get; set; }
        public List<LatLng> Coordinates { get; set; }
        public List<List<LatLng>> Zones { get; set; }
    }

    public class LatLng
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
