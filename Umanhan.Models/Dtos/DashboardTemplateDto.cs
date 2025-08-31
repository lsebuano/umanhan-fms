using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Umanhan.Models.Models;

namespace Umanhan.Models.Dtos;

public partial class DashboardTemplateDto
{
    public Guid TemplateId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string DashboardComponentName { get; set; }
}
