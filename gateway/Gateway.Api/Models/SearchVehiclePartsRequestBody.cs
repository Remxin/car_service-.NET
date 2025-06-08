namespace Gateway.Api.Models;

public class SearchVehiclePartsRequestBody {
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Name { get; set; }
    public string? PartNumber { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public int? AvailableQuantity { get; set; }
}