using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmContractPaymentDetail : IEntity
{
    [Column("ContractPaymentDetailId")]
    public Guid Id { get; set; }

    public Guid ContractPaymentId { get; set; }

    public string Item { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public string Unit { get; set; }

    public virtual FarmContractPayment ContractPayment { get; set; }
}
