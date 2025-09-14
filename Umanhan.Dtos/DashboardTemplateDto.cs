namespace Umanhan.Dtos;

public partial class DashboardTemplateDto : BaseDto
{
    public Guid TemplateId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string DashboardComponentName { get; set; }
}
