using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class FarmNumberSeryDto
    {
        public Guid NumberSeryId { get; set; }

        public Guid FarmId { get; set; }

        public string FarmName { get; set; }

        public string Type { get; set; }

        public int CurrentSery { get; set; }

        public string Format { get; set; }
    }
}
