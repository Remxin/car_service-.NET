namespace WorkshopService.Entities;

public class VehicleEntity {
    public int Id { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public string Vin { get; set; } = default!;
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ServiceOrderEntity> ServiceOrders { get; set; } = new List<ServiceOrderEntity>();
}