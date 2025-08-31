using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos;

public class UserActivityDto
{
    public Guid UserActivityId { get; set; }

    public Guid? FarmId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Username { get; set; }

    public string IpAddress { get; set; }

    public string ModuleName { get; set; }

    public string Path { get; set; }

    public string Properties { get; set; }
}
