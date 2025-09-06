namespace Umanhan.Dtos;

public partial class FarmKpiSummaryDto
{
    public Guid FarmId { get; set; }

    public string KpiName { get; set; }

    public decimal KpiValue { get; set; }

    public string PeriodType { get; set; }

    public DateOnly PeriodDate { get; set; }

    public string SourceRun { get; set; }
}
