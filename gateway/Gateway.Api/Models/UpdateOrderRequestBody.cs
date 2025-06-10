namespace Gateway.Api.Models;

public class UpdateOrderRequestBody {
    public int VehicleId { get; set; }
    public string Status { get; set; }
    public int AssignedMechanicId { get; set; }
}
