namespace WorkshopService.Entities;

public class ServiceCommentEntity {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public ServiceOrderEntity? Order { get; set; }
}