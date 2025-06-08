namespace Gateway.Api.Models;

public class SearchVehiclesRequestBody {
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Vin { get; set; }
}