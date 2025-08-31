using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.LoggerEntities;

public partial class UserActivity : IEntity
{
    [Column("UserActivityId")]
    public Guid Id { get; set; }

    public Guid? FarmId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Username { get; set; }

    public string IpAddress { get; set; }

    public string ModuleName { get; set; }

    public string Path { get; set; }

    public string Properties { get; set; }
}
