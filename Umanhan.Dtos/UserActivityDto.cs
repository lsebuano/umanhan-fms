namespace Umanhan.Dtos;

public class UserActivityDto : BaseDto
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
