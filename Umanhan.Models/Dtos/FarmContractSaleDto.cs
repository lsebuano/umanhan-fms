using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Shared.Extensions;

namespace Umanhan.Models.Dtos
{
    public class FarmContractSaleDto
    {
        public Guid ContractSaleId { get; set; }

        public Guid ContractDetailId { get; set; }

        public Guid UnitId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ProductId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid ProductTypeId { get; set; }

        public string Product { get; set; }

        public string ProductVariety { get; set; }

        public string ProductType { get; set; }

        public string Customer { get; set; }

        public string Unit { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public string TotalAmountCompact => TotalAmount.ToNumberCompact();

        public DateTime Date { get; set; }

        public string Notes { get; set; }
    }
}
