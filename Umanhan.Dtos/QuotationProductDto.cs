namespace Umanhan.Dtos;

public partial class QuotationProductDto : BaseDto
{
    public Guid QuotationProductId { get; set; }

    public Guid QuotationId { get; set; }

    public Guid UnitId { get; set; }

    public Guid ProductTypeId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductTypeName { get; set; }

    public string ProductName { get; set; }

    public string Unit { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total => Quantity * UnitPrice;

    public Guid RowKey => Guid.NewGuid();
}
