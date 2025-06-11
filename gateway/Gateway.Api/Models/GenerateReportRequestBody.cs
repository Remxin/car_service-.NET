using Google.Protobuf.WellKnownTypes;

namespace Gateway.Api.Models;

public class GenerateReportRequestBody {
    public int UserId { get; set; }
    public int OrderId { get; set; }
}