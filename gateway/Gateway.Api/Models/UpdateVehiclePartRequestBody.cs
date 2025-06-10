namespace Gateway.Api.Models;

public class UpdateVehiclePartRequestBody {
    public string Name { get; set; }
    public string PartNumber { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int AvailableQuantity { get; set; }
}