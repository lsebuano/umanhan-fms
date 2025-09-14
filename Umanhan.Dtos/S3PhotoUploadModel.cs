namespace Umanhan.Dtos
{
    public class S3PhotoUploadDto : BaseDto
    {
        public Guid ActivityId { get; set; }
        public string S3ObjectKey { get; set; }
        public string S3ObjectContentType { get; set; }
        public string Notes { get; set; }
    }
}
