namespace Gateway.Api.Models;

public class AddVehicleRequestBody {
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Vin { get; set; } = null!;
    public string CarImageUrl { get; set; } = null!;
    public int Year { get; set; }
}