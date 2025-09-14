namespace Umanhan.Dtos
{
    public class PaymentTypeDto : BaseDto
    {
        public Guid PaymentTypeId { get; set; }

        public string PaymentTypeName { get; set; }

        public IEnumerable<FarmActivityLaborerDto> FarmActivityLaborers { get; set; } = [];
    }
}
