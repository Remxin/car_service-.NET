namespace ReportService.Services;
using Grpc.Core;
using ReportService;
using Shared.Grpc.Messages;

public class ReportServiceImplementation : Shared.Grpc.Services.ReportService.ReportServiceBase {
    // public override async Task<GetReportDownloadLinkResponse> GetReportDownloadLink(GetReportDownloadLinkRequest request,
    //     ServerCallContext context) {
    //     var response = new GetReportDownloadLinkResponse {
    //         DownloadUrl = "",
    //         ExpiresAt = DateTime.Now.AddHours(1),
    //         
    //     }
    // }
    public override async Task<GetReportStatusResponse> GetReportStatus(GetReportStatusRequest request,
        ServerCallContext context) {
        var response = new GetReportStatusResponse {
            ReportId = "0",
            Status = "ERROR",
            ErrorMessage = "aa"
        };

        return response;
    }
}