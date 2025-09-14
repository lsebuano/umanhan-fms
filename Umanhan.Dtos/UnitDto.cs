namespace Umanhan.Dtos
{
    public class UnitDto : BaseDto
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
