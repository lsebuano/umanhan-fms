using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class CustomerType : IEntity
{
    [Column("TypeId")]
    public Guid Id { get; set; }

    public string CustomerTypeName { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = [];
}
