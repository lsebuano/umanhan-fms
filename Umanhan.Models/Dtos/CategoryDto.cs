using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Group { get; set; }

        public string Group2 { get; set; }

        public string ConsumptionBehavior { get; set; }

        public IEnumerable<InventoryDto> Inventories { get; set; } = [];

        public IEnumerable<TaskDto> Tasks { get; set; } = [];
    }
}
