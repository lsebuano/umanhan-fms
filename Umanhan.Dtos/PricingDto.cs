namespace Umanhan.Dtos
{
    public class PricingDto : BaseDto
    {
        public Guid PricingId { get; set; }

        public Guid FarmId { get; set; }

        public Guid ProfileId { get; set; }

        public string FarmName { get; set; }

        public Guid ConditionTypeId { get; set; }

        public string ConditionType { get; set; }

        public bool ConditionIsDeduction { get; set; }

        public string ConditionIsDeductionString => ConditionIsDeduction ? "Deduction" : "Addition";

        public decimal Value { get; set; }

        public string ApplyType { get; set; }

        public int Sequence { get; set; }

        public string ProfileName { get; set; }

        public string ProfileDescription { get; set; }

        public decimal Before { get; set; }
        public decimal Delta { get; set; }
        public decimal After { get; set; }

        public string Symbol => ConditionIsDeduction ? "-" : "+";
        public string Suffix => ApplyType.Equals("PERCENTAGE", StringComparison.OrdinalIgnoreCase) ? "%" : "";
    }
}
