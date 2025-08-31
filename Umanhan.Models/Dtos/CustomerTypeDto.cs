using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class CustomerTypeDto
    {
        public Guid TypeId { get; set; }

        public string CustomerTypeName { get; set; }

        public IEnumerable<CustomerDto> Customers { get; set; } = [];
    }
}
