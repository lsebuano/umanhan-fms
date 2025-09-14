namespace Umanhan.Dtos
{
    public class RolePermissionDto : BaseDto
    {
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public bool RoleIsActive { get; set; }

        public Guid ModuleId { get; set; }

        public string ModuleName { get; set; }

        public Guid PermissionId { get; set; }

        public string PermissionName { get; set; }

        public string Access => $"{ModuleName}.{PermissionName}";
    }
}
