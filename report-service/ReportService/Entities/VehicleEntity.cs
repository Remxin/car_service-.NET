namespace ReportService.Entities;

public class VehicleEntity {
    public int Id { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public string Vin { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}