using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class QuotationProduct : IEntity
{
    [Column("QuotationProductId")]
    public Guid Id { get; set; }

    public Guid QuotationId { get; set; }

    public Guid ProductTypeId { get; set; }

    public Guid ProductId { get; set; }

    public Guid UnitId { get; set; }

    public string ProductName { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }

    public virtual ProductType ProductType { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual Quotation Quotation { get; set; }
}
