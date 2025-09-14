namespace Umanhan.Dtos
{
    public class FarmZoneDto : BaseDto
    {
        public Guid ZoneId { get; set; }

        public Guid FarmId { get; set; }

        public Guid? SoilId { get; set; }

        public string ZoneName { get; set; }

        public string Description { get; set; }

        public decimal? SizeInHectares { get; set; }

        public decimal? AreaInSqm { get; set; }

        public string IrrigationType { get; set; }

        public string SoilType { get; set; }

        public double? ZoneCentroidLat { get; set; }

        public double? ZoneCentroidLng { get; set; }

        public string ZoneBoundaryJson { get; set; }

        public string ZoneColor { get; set; }

        public string FarmZoneNotes { get; set; }

        public string FarmName { get; set; }

        public decimal? FarmSizeInHectares { get; set; }

        public decimal? FarmSizeInSqm { get; set; }

        public string FarmLocation { get; set; }

        public string FarmFullAddress { get; set; }

        public double? FarmCentroidLat { get; set; }

        public double? FarmCentroidLng { get; set; }

        public string FarmBoundaryJson { get; set; }

        public bool FarmSetupComplete { get; set; }

        public string FarmStaticMapUrl { get; set; }


        public Guid CropId { get; set; }
        public string CropName { get; set; }

        public Guid UnitId { get; set; }
        public string UnitName { get; set; }

        public string CropVariety { get; set; }
    }
}
