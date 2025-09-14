namespace Umanhan.Dtos
{
    public class LivestockDto : BaseDto
    {
        public Guid LivestockId { get; set; }

        public string AnimalType { get; set; }

        public string Breed { get; set; }

        public IEnumerable<FarmLivestockDto> FarmLivestocks { get; set; } = [];
    }
}
