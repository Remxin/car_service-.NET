namespace WorkshopService.Entities;

public class ServiceOrderEntity {
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string Status { get; set; } = default!;
    public int? AssignedMechanicId { get; set; }
    public DateTime CreatedAt { get; set; }

    public VehicleEntity? Vehicle { get; set; }
    public ICollection<ServiceTaskEntity> ServiceTasks { get; set; } = new List<ServiceTaskEntity>();
    public ICollection<ServicePartEntity> ServiceParts { get; set; } = new List<ServicePartEntity>();
    public ICollection<ServiceCommentEntity> ServiceComments { get; set; } = new List<ServiceCommentEntity>();
}