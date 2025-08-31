using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos
{
    public class SystemSettingDto
    {
        public Guid SettingId { get; set; }

        public string SettingName { get; set; }

        public string SettingValue { get; set; }
    }
}
