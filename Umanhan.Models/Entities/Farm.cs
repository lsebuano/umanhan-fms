using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Farm : IEntity
{
    [Column("FarmId")]
    public Guid Id { get; set; }

    public string FarmName { get; set; }

    public string Location { get; set; }

    public string FullAddress { get; set; }

    public decimal? SizeInHectares { get; set; }

    public decimal? SizeInSqm { get; set; }

    public string OwnerName { get; set; }

    public decimal? Lat { get; set; }

    public decimal? Lng { get; set; }

    public string BoundaryJson { get; set; }

    public string Notes { get; set; }

    public bool SetupComplete { get; set; }

    public bool SetupStarted { get; set; }

    public string StaticMapUrl { get; set; }
    public string Tin { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = [];

    public virtual ICollection<FarmContract> FarmContracts { get; set; } = [];

    public virtual ICollection<FarmCrop> FarmCrops { get; set; } = [];

    public virtual ICollection<FarmInventory> FarmInventories { get; set; } = [];

    public virtual ICollection<FarmLivestock> FarmLivestocks { get; set; } = [];

    public virtual ICollection<FarmZone> FarmZones { get; set; } = [];

    public virtual ICollection<Staff> Staffs { get; set; } = [];

    public virtual ICollection<FarmTransaction> FarmTransactions { get; set; } = [];

    public virtual ICollection<FarmProduceInventory> FarmProduceInventories { get; set; } = [];

    public virtual ICollection<PricingProfile> PricingProfiles { get; set; } = [];

    public virtual ICollection<Quotation> Quotations { get; set; } = [];

    public virtual ICollection<FarmGeneralExpense> FarmGeneralExpenses { get; set; } = [];

    public virtual ICollection<FarmZoneYield> FarmZoneYields { get; set; } = [];
    public virtual ICollection<FarmNumberSery> FarmNumberSeries { get; set; } = new List<FarmNumberSery>();
}
