namespace Umanhan.Dtos
{
    public class TransactionTypeDto : BaseDto
    {
        public Guid TypeId { get; set; }

        public string TransactionTypeName { get; set; }

        public IEnumerable<FarmTransactionDto> FarmTransactions { get; set; } = [];
    }
}
