using System.Text.Json.Serialization;
using Umanhan.Shared;

namespace Umanhan.Dtos
{
    public class FarmActivityDto
    {
        public Guid ActivityId { get; set; }

        public Guid TaskId { get; set; }

        public Guid ProductId { get; set; }

        public Guid FarmId { get; set; }

        public Guid SupervisorId { get; set; }

        [JsonIgnore]
        public DateOnly StartDateUtc { get; set; }

        public DateOnly StartDate => StartDateUtc.ToManilaTime();

        public DateTime StartDateTime { get; set; }

        [JsonIgnore]
        public DateOnly? EndDateUtc { get; set; }

        public DateOnly? EndDate => EndDateUtc?.ToManilaTime();

        public DateTime? EndDateTime { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }

        public Guid ProductTypeId { get; set; }

        public Guid? ContractId { get; set; }

        public string ProductType { get; set; }

        public string Supervisor { get; set; }

        public string Task { get; set; }

        public string FarmName { get; set; }

        public FarmContractDto Contract { get; set; }

        public FarmDto Farm { get; set; }

        public IEnumerable<FarmActivityExpenseDto> FarmActivityExpenses { get; set; } = [];

        public IEnumerable<FarmActivityLaborerDto> FarmActivityLaborers { get; set; } = [];

        public IEnumerable<FarmActivityUsageDto> FarmActivityUsages { get; set; } = [];

        public IEnumerable<FarmProduceInventoryDto> FarmProduceInventories { get; set; } = [];

        public Guid[] SelectedFarmTaskIds { get; set; } = [];
    }
}
