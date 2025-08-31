using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmContractSale : IEntity
{
    [Column("ContractSaleId")]
    public Guid Id { get; set; }

    public Guid ContractDetailId { get; set; }

    public Guid ProductTypeId { get; set; }

    public Guid ProductId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid UnitId { get; set; }

    public Guid FarmId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public DateOnly Date { get; set; }

    public string ProductName { get; set; }

    public string ProductVariety { get; set; }

    public string ProductTypeName { get; set; }

    public string CustomerName { get; set; }

    public string UnitName { get; set; }

    public string Notes { get; set; }

    public virtual FarmContractDetail ContractDetail { get; set; }
}
