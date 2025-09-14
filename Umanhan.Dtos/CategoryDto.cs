namespace Umanhan.Dtos
{
    public class CategoryDto : BaseDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Group { get; set; }

        public string Group2 { get; set; }

        public string ConsumptionBehavior { get; set; }

        public IEnumerable<InventoryDto> Inventories { get; set; } = [];

        public IEnumerable<TaskDto> Tasks { get; set; } = [];
    }
}
