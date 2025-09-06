namespace Umanhan.Dtos
{
    public class RoleClaimDto
    {
        public IEnumerable<RolePermissionDto> Permissions { get; set; } = [];
        public string DashboardComponent { get; set; }
    }
}
