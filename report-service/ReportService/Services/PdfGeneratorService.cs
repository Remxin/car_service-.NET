using ReportService.Entities;

namespace ReportService.Services;

public interface IPdfGeneratorService {
    string GenerateReportPdf(string title, ReportEntity reportEntity);
}

public class PdfGeneratorService(string folderPath) : IPdfGeneratorService {
    private string _folderPath = folderPath;

    public string GenerateReportPdf(string title, ReportEntity reportEntity) {
        throw new NotImplementedException();
        Directory.CreateDirectory(_folderPath);
        var filePath = Path.Combine(_folderPath, title + ".pdf");
    }
}