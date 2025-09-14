namespace Umanhan.Dtos
{
    public class FarmGeneralExpenseReceiptDto : BaseDto
    {
        public Guid ReceiptId { get; set; }

        public Guid GeneralExpenseId { get; set; }

        public string ReceiptUrlThumbnail { get; set; }

        public string ReceiptUrlFull { get; set; }

        public string MimeType { get; set; }

        public string Notes { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
