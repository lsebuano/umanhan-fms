using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Shared
{
    public enum ContractStatus
    {
        NEW = 0,
        HARVEST_SCHEDULED = 1,
        HARVESTED = 3,
        PICKUP_SCHEDULED = 7,
        PICKED_UP = 8,
        PAID = 9,

        CANCELLED = 2,
        PARTIALLY_PAID = 4,
        PICKUP_CONFIRMED = 5,
        RECOVERED = 6,
    }
}
