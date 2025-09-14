using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.LoggerEntities;

public partial class EfQueryLog : IEntity
{
    [Column("QueryId")]
    public Guid Id { get; set; }

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
