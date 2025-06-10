namespace Gateway.Api.Models;

public class UpdateVehicleRequestBody {
    public string CarImageUrl { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Vin { get; set; }
}