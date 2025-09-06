namespace Umanhan.Dtos
{
    public class ProductTypeDto
    {
        public Guid TypeId { get; set; }

        public string ProductTypeName { get; set; }

        public string TableName { get; set; }

        public IEnumerable<FarmActivityDto> FarmActivities { get; set; } = [];

        public IEnumerable<FarmContractDetailDto> FarmContractDetails { get; set; } = [];

        public IEnumerable<FarmProduceInventoryDto> FarmProduceInventories { get; set; } = [];

        public IEnumerable<FarmTransactionDto> FarmTransactions { get; set; } = [];
    }
}
