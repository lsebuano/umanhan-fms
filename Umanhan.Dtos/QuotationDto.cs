namespace Umanhan.Dtos;

public partial class QuotationDto
{
    public Guid QuotationId { get; set; }

    public string QuotationNumber { get; set; }

    public DateTime DateSent { get; set; }

    public string RecipientEmail { get; set; }

    public string RecipientName { get; set; }

    public decimal BasePrice { get; set; }

    public decimal FinalPrice { get; set; }

    public DateOnly ValidUntil { get; set; }

    public Guid? PricingProfileId { get; set; }

    public string Status { get; set; }

    public Guid FarmId { get; set; }

    public string FarmName { get; set; }

    public string AwsSesMessageId { get; set; }

    public decimal QuotationTotal => QuotationProducts.Sum(x => x.Total);

    public virtual ICollection<QuotationProductDto> QuotationProducts { get; set; } = [];
}
