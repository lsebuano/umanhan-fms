namespace Umanhan.Dtos
{
    public class FarmActivityPhotoDto : BaseDto
    {
        public Guid PhotoId { get; set; }

        public Guid ActivityId { get; set; }

        public string PhotoUrlThumbnail { get; set; }

        public string PhotoUrlFull { get; set; }

        public string MimeType { get; set; }

        public string Notes { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
