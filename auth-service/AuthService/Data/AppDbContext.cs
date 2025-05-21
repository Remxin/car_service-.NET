using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Models;

namespace AuthService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<Role>().ToTable("roles");
        modelBuilder.Entity<Permission>().ToTable("permissions");
        modelBuilder.Entity<UserRole>().ToTable("user_roles");
        modelBuilder.Entity<RolePermission>().ToTable("role_permissions");

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp without time zone");
        });
    }
}