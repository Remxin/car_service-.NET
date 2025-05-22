namespace AuthService.Entities;

public class RolePermissionEntity
{
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public PermissionEntity Permission { get; set; } = null!;
}