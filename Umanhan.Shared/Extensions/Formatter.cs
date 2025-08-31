using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Shared.Extensions
{
    public static class Formatter
    {
        public static string ToNumberCompact(this decimal number, string format = "n2")
        {
            decimal absNumber = Math.Abs(number); // Support negative values if needed

            //if (absNumber < 1_000)
            //    return number.ToString(format);

            //if (absNumber < 1_000_000)
            //    return (number / 1_000M).ToString(format) + "K";

            if (absNumber < 1_000_000)
                return number.ToString(format);

            if (absNumber < 1_000_000_000)
                return (number / 1_000_000M).ToString(format) + "M";

            if (absNumber < 1_000_000_000_000)
                return (number / 1_000_000_000M).ToString(format) + "B";

            return (number / 1_000_000_000_000M).ToString(format) + "T";
        }


    }
}
