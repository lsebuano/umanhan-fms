namespace Umanhan.Dtos
{
    public class RoleClaimDto : BaseDto
    {
        public IEnumerable<RolePermissionDto> Permissions { get; set; } = [];
        public string DashboardComponent { get; set; }
        public string FarmId { get; set; }
    }
}
