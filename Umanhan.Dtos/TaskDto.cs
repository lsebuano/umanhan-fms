namespace Umanhan.Dtos
{
    public class TaskDto : BaseDto
    {
        public Guid TaskId { get; set; }

        public string TaskName { get; set; }

        public Guid? CategoryId { get; set; }

        public string Category { get; set; }

        public string CategoryGroup { get; set; }

        public string CategoryGroup2 { get; set; }

        public string CategoryConsumptionBehavior { get; set; }

        public IEnumerable<FarmActivityDto> FarmActivities { get; set; } = [];
    }
}
