using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Models;

namespace AuthService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
    public DbSet<RolePermissionEntity> RolePermissions => Set<RolePermissionEntity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<RolePermissionEntity>(entity =>
        {
            entity.ToTable("role_permissions");

            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.Property(rp => rp.RoleId).HasColumnName("role_id");
            entity.Property(rp => rp.PermissionId).HasColumnName("permission_id");

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        });


        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.Name).HasColumnName("name");
            entity.Property(r => r.Description).HasColumnName("description");
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Name).HasColumnName("name");
            entity.Property(p => p.Description).HasColumnName("description");
        });

        modelBuilder.Entity<UserRoleEntity>(entity =>
        {
            entity.ToTable("user_roles");

            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.Property(ur => ur.UserId).HasColumnName("user_id");
            entity.Property(ur => ur.RoleId).HasColumnName("role_id");

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });


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