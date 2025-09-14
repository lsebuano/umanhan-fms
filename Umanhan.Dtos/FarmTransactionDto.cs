namespace Umanhan.Dtos
{
    public class FarmTransactionDto : BaseDto
    {
        public Guid TransactionId { get; set; }

        public Guid ProduceInventoryId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductTypeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid UnitId { get; set; }

        public Guid? BuyerId { get; set; }

        public DateTime Date { get; set; }

        public string Notes { get; set; }

        public Guid TransactionTypeId { get; set; }

        public string BuyerName { get; set; }

        public string Product { get; set; }

        public string ProductVariety { get; set; }

        public string ProductType { get; set; }

        public string TransactionType { get; set; }

        public string Unit { get; set; }

        public Guid FarmId { get; set; }

        public string PaymentType { get; set; }
    }
}
