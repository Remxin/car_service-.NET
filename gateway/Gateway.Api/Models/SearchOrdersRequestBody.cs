namespace Gateway.Api.Models;

public class SearchOrdersRequestBody {
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? VehicleVin { get; set; }
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public int? VehicleYear { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}