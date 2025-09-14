namespace Umanhan.Dtos
{
    public class SystemSettingDto : BaseDto
    {
        public Guid SettingId { get; set; }

        public string SettingName { get; set; }

        public string SettingValue { get; set; }
    }
}
