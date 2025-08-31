using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.LoggerEntities;

public partial class Log : IEntity
{
    [Column("LogId")]
    public Guid Id { get; set; }

    public Guid? FarmId { get; set; }

    public DateTime Timestamp { get; set; }

    /// <summary>
    /// INFO, DEBUG, ERROR
    /// </summary>
    public string Level { get; set; }

    public string Message { get; set; }

    public string Exception { get; set; }

    public string Properties { get; set; }
}
