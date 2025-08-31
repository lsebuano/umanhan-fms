using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Quotation : IEntity
{
    [Column("QuotationId")]
    public Guid Id { get; set; }

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

    public string AwsSesMessageId { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual PricingProfile? PricingProfile { get; set; }

    public virtual ICollection<QuotationDetail> QuotationDetails { get; set; } = [];

    public virtual ICollection<QuotationProduct> QuotationProducts { get; set; } = [];
}
