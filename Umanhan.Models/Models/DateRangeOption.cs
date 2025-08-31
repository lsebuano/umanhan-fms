using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class DateRangeOption(string text, string value)
    {
        private readonly string _text = text;
        private readonly string _value = value;

        public string Text => _text;
        public string Value => _value;
    }
}
