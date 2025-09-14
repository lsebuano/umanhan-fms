namespace Umanhan.Dtos
{
    public class PricingConditionTypeDto : BaseDto
    {
        public Guid ConditionId { get; set; }

        public string Name { get; set; }

        public bool IsDeduction { get; set; }
    }
}
