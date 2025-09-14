namespace Umanhan.Dtos
{
    public class FarmActivityLaborerDto : BaseDto
    {
        public Guid LaborActivityId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid LaborerId { get; set; }

        public Guid PaymentTypeId { get; set; }

        public decimal Rate { get; set; }

        public short QuantityWorked { get; set; }

        public string PaymentType { get; set; }

        public string LaborName { get; set; }

        public decimal TotalPayment { get; set; }

        public DateTime Timestamp { get; set; }

        public void Recompute()
        {
            if (string.Equals(PaymentType, "CONTRACT", StringComparison.OrdinalIgnoreCase))
            {
                TotalPayment = Rate;
            }
            else
            {
                TotalPayment = Rate * QuantityWorked;
            }
        }
    }
}
