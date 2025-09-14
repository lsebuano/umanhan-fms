namespace Umanhan.Dtos
{
    public class CropDto : BaseDto
    {
        public Guid CropId { get; set; }

        public string CropName { get; set; }

        public string CropVariety { get; set; }

        public string Notes { get; set; }

        public decimal? DefaultRatePerUnit { get; set; }

        public Guid DefaultUnitId { get; set; }

        public string DefaultUnit { get; set; }

        public IEnumerable<FarmCropDto> FarmCrops { get; set; } = [];

        public IEnumerable<FarmZoneYieldDto> FarmZoneYields { get; set; } = [];
    }
}
