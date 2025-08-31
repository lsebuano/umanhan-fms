using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class QuotationDetail : IEntity
{
    [Column("DetailId")]
    public Guid Id { get; set; }

    public Guid QuotationId { get; set; }

    public decimal Sequence { get; set; }

    public string ApplyType { get; set; }

    public decimal Value { get; set; }

    public bool IsDeduction { get; set; }

    public string ConditionType { get; set; }

    public decimal Before { get; set; }

    public decimal Delta { get; set; }

    public decimal After { get; set; }

    public virtual Quotation Quotation { get; set; }
}
