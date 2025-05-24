using Microsoft.EntityFrameworkCore;
using WorkshopService.Entities;

namespace WorkshopService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<VehicleEntity> Vehicles => Set<VehicleEntity>();
    public DbSet<ServiceOrderEntity> ServiceOrders => Set<ServiceOrderEntity>();
    public DbSet<ServiceTaskEntity> ServiceTasks => Set<ServiceTaskEntity>();
    public DbSet<ServicePartEntity> ServiceParts => Set<ServicePartEntity>();
    public DbSet<VehiclePartEntity> VehicleParts => Set<VehiclePartEntity>();
    public DbSet<ServiceCommentEntity> ServiceComments => Set<ServiceCommentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<VehicleEntity>(entity => {
            entity.ToTable("vehicles");
            entity.HasKey(e => e.Id);
            entity.Property(v => v.Id).HasColumnName("id");
            entity.Property(v => v.Brand).HasColumnName("brand");
            entity.Property(v => v.Model).HasColumnName("model");
            entity.Property(v => v.Year).HasColumnName("year");
            entity.Property(v => v.Vin).HasColumnName("vin");
            entity.HasIndex(v => v.Vin).IsUnique();
            entity.Property(v => v.PhotoUrl).HasColumnName("photo_url");
            entity.Property(v => v.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp without time zone");
        });
        modelBuilder.Entity<ServiceOrderEntity>(entity =>
        {
            entity.ToTable("service_orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.AssignedMechanicId).HasColumnName("assigned_mechanic_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp without time zone");

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.ServiceOrders)
                .HasForeignKey(e => e.VehicleId);
        });
        
        modelBuilder.Entity<ServiceTaskEntity>(entity =>
        {
            entity.ToTable("service_tasks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LaborCost).HasColumnName("labor_cost").HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp without time zone");

            entity.HasOne(e => e.Order)
                .WithMany(o => o.ServiceTasks)
                .HasForeignKey(e => e.OrderId);
        });
        
        modelBuilder.Entity<ServicePartEntity>(entity =>
        {
            entity.ToTable("service_parts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.VehiclePartId).HasColumnName("vehicle_part_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(e => e.Order)
                .WithMany(o => o.ServiceParts)
                .HasForeignKey(e => e.OrderId);

            entity.HasOne(e => e.VehiclePart)
                .WithMany(p => p.ServiceParts)
                .HasForeignKey(e => e.VehiclePartId);
        });
        
        modelBuilder.Entity<VehiclePartEntity>(entity =>
        {
            entity.ToTable("vehicle_parts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.PartNumber).HasColumnName("part_number");
            entity.HasIndex(e => e.PartNumber).IsUnique();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
            entity.Property(e => e.AvailableQuantity).HasColumnName("available_quantity");
        });

        modelBuilder.Entity<ServiceCommentEntity>(entity =>
        {
            entity.ToTable("service_comments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp without time zone");

            entity.HasOne(e => e.Order)
                .WithMany(o => o.ServiceComments)
                .HasForeignKey(e => e.OrderId);
        });
    }
}