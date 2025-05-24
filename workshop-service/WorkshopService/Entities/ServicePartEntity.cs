namespace WorkshopService.Entities;

public class ServicePartEntity {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int VehiclePartId { get; set; }
    public int Quantity { get; set; }

    public ServiceOrderEntity? Order { get; set; }
    public VehiclePartEntity? VehiclePart { get; set; }
}