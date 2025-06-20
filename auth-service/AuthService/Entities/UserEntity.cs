﻿namespace AuthService.Entities;

public class UserEntity  {
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
}
