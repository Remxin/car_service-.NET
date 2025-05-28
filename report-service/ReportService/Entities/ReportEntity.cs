namespace ReportService.Entities;

public class ReportEntity {
    public int Order_id { get; set; }
    public int VehicleId { get; set; }
    public string Status { get; set; }
    public UserEntity User { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ReportUrl { get; set; }

    public VehicleEntity? Vehicle { get; set; }
    public ICollection<ServiceTaskEntity> ServiceTasks { get; set; } = new List<ServiceTaskEntity>();
    public ICollection<ServicePartEntity> ServiceParts { get; set; } = new List<ServicePartEntity>();
    public ICollection<ServiceCommentEntity> ServiceComments { get; set; } = new List<ServiceCommentEntity>();
}