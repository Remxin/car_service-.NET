namespace ReportService.Entities;


public class ServiceTaskEntity {
    public int Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal LaborCost { get; set; }
    public DateTime CreatedAt { get; set; } 
}