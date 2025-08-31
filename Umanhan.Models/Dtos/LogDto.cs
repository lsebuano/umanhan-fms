using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umanhan.Models.Dtos;

public class LogDto
{
    public Guid LogId { get; set; }

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
