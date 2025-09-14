namespace Umanhan.Dtos;

public class QueryLogDto : BaseDto
{
    public Guid QueryId { get; set; }

    public Guid? CorrelationId { get; set; }

    public Guid? FarmId { get; set; }

    public string ApiEndpoint { get; set; }

    public string HttpMethod { get; set; }

    public string Query { get; set; }

    public string Parameters { get; set; }

    public int DurationMs { get; set; }

    public int? RowsReturned { get; set; }

    public Guid? UserId { get; set; }

    public string Source { get; set; }

    public string Environment { get; set; }

    public DateTime? CreatedAt { get; set; }
}
