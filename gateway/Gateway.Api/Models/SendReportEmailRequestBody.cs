namespace Gateway.Api.Models;

public class SendReportEmailRequestBody {
    public int ReportId { get; set; }
    public List<int> UsersIds { get; set; }
}