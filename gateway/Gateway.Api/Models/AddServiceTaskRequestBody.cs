namespace Gateway.Api.Models;

public class AddServiceTaskRequestBody {
    public int OrderId { get; set; }
    public string Description { get; set; } = null!;
    public double LaborCost { get; set; }
}