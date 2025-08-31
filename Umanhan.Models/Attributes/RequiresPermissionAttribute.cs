namespace Umanhan.Models.Attributes
{
    public class RequiresPermissionAttribute : Attribute
    {
        public string PermissionName { get; }
        public RequiresPermissionAttribute(string permissionName)
        {
            PermissionName = permissionName;
        }
    }
}
