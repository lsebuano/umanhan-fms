using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmContractPayment : IEntity
{
    [Column("ContractPaymentId")]
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public string FarmName { get; set; }

    public string FarmAddress { get; set; }

    public string FarmTin { get; set; }

    public string FarmLogo { get; set; }

    public string FarmContactPhone { get; set; }

    public string FarmContactEmail { get; set; }

    public string DocumentType { get; set; }

    public string InvoiceNo { get; set; }

    public DateOnly Date { get; set; }

    public string BuyerName { get; set; }

    public string BuyerTin { get; set; }

    public string BuyerAddress { get; set; }

    public string BuyerContactNo { get; set; }

    public decimal TotalAmountReceived { get; set; }

    public decimal TotalAdjustments { get; set; }

    public string AmountInWords { get; set; }

    public string ReceivedBy { get; set; }

    public string PrintedBy { get; set; }

    public DateTime? PrintTimestamp { get; set; }

    public string SystemRefNo { get; set; }

    public string PaymentMethod { get; set; }

    public string PaymentRef { get; set; }

    public decimal Subtotal { get; set; }

    public virtual ICollection<FarmContractPaymentDetail> FarmContractPaymentDetails { get; set; } = new List<FarmContractPaymentDetail>();
}
