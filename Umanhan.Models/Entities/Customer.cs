using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class Customer : IEntity
{
    [Column("CustomerId")]
    public Guid Id { get; set; }

    public string CustomerName { get; set; }

    public string Address { get; set; }

    public string ContactInfo { get; set; }

    public string EmailAddress { get; set; }

    public string Tin { get; set; }

    public Guid CustomerTypeId { get; set; }

    public bool ContractEligible { get; set; }

    public virtual CustomerType CustomerType { get; set; }

    public virtual ICollection<FarmContract> FarmContracts { get; set; } = [];

    public virtual ICollection<FarmTransaction> FarmTransactions { get; set; } = [];
}
