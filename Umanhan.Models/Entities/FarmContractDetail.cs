using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmContractDetail : IEntity
{
    [Column("ContractDetailId")]
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public Guid ProductId { get; set; }

    public Guid ProductTypeId { get; set; }

    public Guid? PricingProfileId { get; set; }

    public decimal ContractedQuantity { get; set; }

    public decimal DeliveredQuantity { get; set; }

    public decimal ContractedUnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid UnitId { get; set; }

    public string Status { get; set; }

    public DateOnly? HarvestDate { get; set; }

    public DateOnly? PickupDate { get; set; }

    public DateOnly? PaidDate { get; set; }

    public bool PickupConfirmed { get; set; }

    public bool IsRecovered { get; set; }

    public virtual FarmContract Contract { get; set; }

    public virtual ProductType ProductType { get; set; }

    public virtual Unit Unit { get; set; }

    public virtual PricingProfile PricingProfile { get; set; }

    public virtual ICollection<FarmContractSale> FarmContractSales { get; set; } = [];

    public virtual ICollection<FarmZoneYield> FarmZoneYields { get; set; } = [];
}
