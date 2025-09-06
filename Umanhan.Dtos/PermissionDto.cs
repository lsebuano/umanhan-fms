namespace Umanhan.Dtos
{
    public class PermissionDto
    {
        public Guid PermissionId { get; set; }

        public string Name { get; set; }

        public IEnumerable<RolePermissionDto> RolePermissions { get; set; } = [];
    }
}
