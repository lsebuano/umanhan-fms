namespace Umanhan.Dtos
{
    public class TransactionTypeDto
    {
        public Guid TypeId { get; set; }

        public string TransactionTypeName { get; set; }

        public IEnumerable<FarmTransactionDto> FarmTransactions { get; set; } = [];
    }
}
