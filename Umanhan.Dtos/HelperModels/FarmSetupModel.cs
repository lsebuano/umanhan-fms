namespace Umanhan.Dtos.HelperModels
{
    public class FarmSetupModel
    {
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public string FarmLocation { get; set; }
        public string FarmFullAddress { get; set; }
        public bool IsFarmBoundaryDrawn { get; set; }
        public bool IsZoneBoundariesDrawn { get; set; }
        public bool IsFarmSetupComplete { get; set; }
        public double FarmSizeSqm { get; set; }
        public double FarmSizeHA { get; set; }
        public string FarmStaticMapUrl { get; set; }
        public LatLng FarmCentroid { get; set; }
        public Dictionary<string, IEnumerable<LatLng>> Zones { get; set; }
        //public Dictionary<Guid, Guid> FarmZoneCrops { get; set; }
        public List<FarmZoneDto> FarmZones { get; set; }
        public IEnumerable<LatLng> FarmBoundary { get; set; }
        public List<FarmCropDto> FarmCrops { get; set; }

        public bool Step1Complete { get; set; }
        public bool Step2Complete { get; set; }

        public bool IsFarmCropsSet { get; set; }

        public string OwnerName { get; set; }
        public string Tin { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
    }
}