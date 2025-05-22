namespace AuthService.Entities;

public class UserRoleEntity
{
    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
}