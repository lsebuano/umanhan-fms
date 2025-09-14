using Umanhan.Shared.Extensions;

namespace Umanhan.Dtos
{
    public class FarmContractSaleDto : BaseDto
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
