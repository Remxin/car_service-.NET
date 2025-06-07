namespace WorkshopService.Entities;

public class VehiclePartEntity {
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string PartNumber { get; set; } = default!;
    public string? Description { get; set; }
    public double? Price { get; set; }
    public int? AvailableQuantity { get; set; }

    public ICollection<ServicePartEntity> ServiceParts { get; set; } = new List<ServicePartEntity>();
}