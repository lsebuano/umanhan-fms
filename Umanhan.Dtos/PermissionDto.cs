namespace Umanhan.Dtos
{
    public class PermissionDto : BaseDto
    {
        public Guid PermissionId { get; set; }

        public string Name { get; set; }

        public IEnumerable<RolePermissionDto> RolePermissions { get; set; } = [];
    }
}
