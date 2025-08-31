using System;
using System.Collections.Generic;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class FarmKpiSummary : IEntity
{
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public string KpiName { get; set; }

    public decimal KpiValue { get; set; }

    public string PeriodType { get; set; }

    public DateOnly PeriodDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string SourceRun { get; set; }
}
