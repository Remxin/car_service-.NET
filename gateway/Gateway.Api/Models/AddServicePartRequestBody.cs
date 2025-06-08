namespace Gateway.Api.Models;

public class AddServicePartRequestBody {
    public int OrderId { get; set; }
    public int VehiclePartId { get; set; }
    public int Quantity { get; set; }
}