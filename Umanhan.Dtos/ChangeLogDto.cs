namespace Umanhan.Dtos;

public class ChangeLogDto : BaseDto
{
    public Guid ChangeId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Username { get; set; }

    public string Type { get; set; }

    public string Entity { get; set; }

    public string EntityId { get; set; }

    public string Field { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }
}
