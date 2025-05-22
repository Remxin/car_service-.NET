namespace AuthService.Entities;

public class PermissionEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
}