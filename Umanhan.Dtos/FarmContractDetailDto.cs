using Umanhan.Shared;

namespace Umanhan.Dtos
{
    public class FarmContractDetailDto
    {
        public Guid ContractDetailId { get; set; }

        public Guid ContractId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductTypeId { get; set; }

        public Guid HarvestActivityId { get; set; }

        public Guid? PricingProfileId { get; set; }

        public string Product { get; set; }

        public decimal ContractedQuantity { get; set; }

        public decimal DeliveredQuantity { get; set; }

        public decimal ContractedUnitPrice { get; set; }

        public decimal TotalAmount => ContractedQuantity * ContractedUnitPrice;

        public Guid UnitId { get; set; }

        public string Status { get; set; }

        public string ProductType { get; set; }

        public string Unit { get; set; }

        public DateTime? HarvestDate { get; set; }

        public DateTime? PickupDate { get; set; }

        public DateTime? PaidDate { get; set; }

        public bool PickupConfirmed { get; set; }

        public bool IsRecovered { get; set; }

        public bool IsPaid => ContractStatus.PAID.ToString().Equals(Status, StringComparison.OrdinalIgnoreCase);

        public bool IsCancelled => ContractStatus.CANCELLED.ToString().Equals(Status, StringComparison.OrdinalIgnoreCase);

        public bool HasHarvestActivity { get; set; }

        public string ContractStatus2 { get; set; }
    }
}
