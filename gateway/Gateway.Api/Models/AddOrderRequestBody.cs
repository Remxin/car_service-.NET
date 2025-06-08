namespace Gateway.Api.Models;

public class AddOrderRequestBody {
    public int VehicleId { get; set; }
    public string Status { get; set; } = null!;
    public int AssignedMechanicId { get; set; }
}