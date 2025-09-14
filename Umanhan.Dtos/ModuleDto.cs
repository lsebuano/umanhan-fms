namespace Umanhan.Dtos
{
    public class ModuleDto : BaseDto
    {
        public Guid ModuleId { get; set; }

        public string Name { get; set; }

        public IEnumerable<RolePermissionDto> RolePermissions { get; set; } = [];
    }
}
