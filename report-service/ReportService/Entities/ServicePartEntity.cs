namespace ReportService.Entities;

public class ServicePartEntity {
    public int Id { get; set; }
    public VehiclePartEntity VehiclePart { get; set; }
    public int Quantity { get; set; }
}