using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos;

public partial class QuotationDetailDto
{
    public Guid DetailId { get; set; }

    public Guid QuotationId { get; set; }

    public decimal Sequence { get; set; }

    public string ApplyType { get; set; }

    public decimal Value { get; set; }

    public bool IsDeduction { get; set; }

    public string ConditionType { get; set; }

    public string Sign { get; set; }

    public string Suffix { get; set; }

    public decimal Before { get; set; }

    public decimal Delta { get; set; }

    public decimal After { get; set; }
}
