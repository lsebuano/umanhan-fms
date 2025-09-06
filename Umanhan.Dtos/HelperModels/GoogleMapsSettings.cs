namespace Umanhan.Dtos.HelperModels
{
    public class GoogleMapsSettings
    {
        public string ApiKey { get; set; }
    }

    public class GoogleMapsSettingsWrapper
    {
        public GoogleMapsSettings GoogleMaps { get; set; }
    }
}
