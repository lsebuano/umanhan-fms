namespace Umanhan.Dtos
{
    public class RoleDto : BaseDto
    {
        public Guid RoleId { get; set; }

        public string GroupName { get; set; }

        public bool IsActive { get; set; }

        public Guid? TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string DashboardTemplateComponentName { get; set; }

        public IEnumerable<RolePermissionDto> RolePermissions { get; set; } = [];
    }
}
