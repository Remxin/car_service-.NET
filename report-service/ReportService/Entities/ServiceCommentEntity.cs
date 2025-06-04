namespace ReportService.Entities;


public class ServiceCommentEntity {
    public int Id { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
}