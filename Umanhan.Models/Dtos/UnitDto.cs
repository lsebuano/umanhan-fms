using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Models.Dtos
{
    public class UnitDto
    {
        public Guid UnitId { get; set; }

        public string UnitName { get; set; }

        public IEnumerable<CropDto> Crops { get; set; } = [];

        public IEnumerable<FarmContractDetailDto> FarmContractDetails { get; set; } = [];

        public IEnumerable<FarmContractSaleDto> FarmContractSales { get; set; } = [];

        public IEnumerable<FarmCropDto> FarmCrops { get; set; } = [];

        public IEnumerable<FarmInventoryDto> FarmInventories { get; set; } = [];

        public IEnumerable<FarmTransactionDto> FarmTransactions { get; set; } = [];

        public IEnumerable<InventoryDto> Inventories { get; set; } = [];
    }
}
