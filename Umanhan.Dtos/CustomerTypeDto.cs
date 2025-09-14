namespace Umanhan.Dtos
{
    public class CustomerTypeDto : BaseDto
    {
        public Guid TypeId { get; set; }

        public string CustomerTypeName { get; set; }

        public IEnumerable<CustomerDto> Customers { get; set; } = [];
    }
}
