using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

/// <summary>
/// Transaction records from the produce inventory
/// </summary>
public partial class FarmTransaction : IEntity
{
    [Column("TransactionId")]
    public Guid Id { get; set; }

    /// <summary>
    /// produce inventory id
    /// </summary>
    public Guid ProduceInventoryId { get; set; }

    public Guid FarmId { get; set; }

    public Guid ProductId { get; set; }

    public Guid ProductTypeId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid UnitId { get; set; }

    public Guid? BuyerId { get; set; }

    public DateOnly Date { get; set; }

    public string Notes { get; set; }

    public Guid TransactionTypeId { get; set; }

    public string BuyerName { get; set; }

    public string PaymentType { get; set; }

    public virtual Customer Buyer { get; set; }

    public virtual FarmProduceInventory ProduceInventory { get; set; }

    public virtual ProductType ProductType { get; set; }

    public virtual TransactionType TransactionType { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual Farm Farm { get; set; }
}
