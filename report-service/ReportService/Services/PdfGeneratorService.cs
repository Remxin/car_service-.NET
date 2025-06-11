using System.Globalization;
using ReportService.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using QuestPDF.Drawing;

namespace ReportService.Services;

public interface IPdfGeneratorService {
    string GenerateReportPdf(string title, ReportEntity reportEntity);
}

public class PdfGeneratorService : IPdfGeneratorService {
    private readonly string _folderPath;
    private readonly ILogger<PdfGeneratorService> _logger;
    private static readonly string EmojiFontFile = "NotoColorEmoji-Regular.ttf";

    public PdfGeneratorService(string folderPath, ILogger<PdfGeneratorService> logger) {
        _folderPath = folderPath;
        _logger = logger;
        Directory.CreateDirectory(_folderPath);
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pl-PL");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pl-PL");
        var emojiFontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts",  EmojiFontFile);

        if (File.Exists(emojiFontPath)) {
            try {

                FontManager.RegisterFont(File.OpenRead(emojiFontPath));
                _logger.LogInformation("Successfully registered emoji font from: {EmojiFontPath}", emojiFontPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register emoji font from {EmojiFontPath}. Emojis might not display correctly.", emojiFontPath);
            }
        }
        else {
            _logger.LogWarning("Emoji font file not found at: {EmojiFontPath}. Falling back to system fonts, emojis might not display correctly.", emojiFontPath);
        }
    }

    public string GenerateReportPdf(string title, ReportEntity report)
    {
        var fileName = $"{title}_{report.OrderId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(_folderPath, fileName);
        
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("DejaVu Sans", Fonts.Arial, "Noto Color Emoji"));
           

                page.Header()
                    .Text($"Service Report: {title}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Item().Text($"🔧 Order #{report.OrderId} — Status: {report.Status}");
                    column.Item().Text($"🚗 Vehicle: {report.Vehicle?.Brand} {report.Vehicle?.Model} ({report.Vehicle?.Vin})");
                    column.Item().Text($"👤 Mechanic: {report.User?.Name} — {report.User?.Email}");
                    column.Item().Text($"📅 Created at: {report.CreatedAt:yyyy-MM-dd HH:mm}");

                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                    if (report.ServiceTasks.Any())
                    {
                        column.Item().PaddingTop(10).Text("🛠 Service Tasks:").Bold();
                        foreach (var task in report.ServiceTasks)
                        {
                            column.Item().Text($"- {task.Description} (Labor cost: {task.LaborCost:C})");
                        }
                    }

                    if (report.ServiceParts.Any())
                    {
                        column.Item().PaddingTop(10).Text("🔩 Parts Used:").Bold();
                        foreach (var part in report.ServiceParts)
                        {
                            column.Item().Text($"- {part.VehiclePart?.Name} x{part.Quantity} – Unit price: {part.VehiclePart?.Price:C}");
                        }
                    }

                    if (report.ServiceComments.Any())
                    {
                        column.Item().PaddingTop(10).Text("💬 Comments:").Bold();
                        foreach (var comment in report.ServiceComments)
                        {
                            column.Item().Text($"- {comment.CreatedAt:yyyy-MM-dd HH:mm}: {comment.Content}");
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated automatically on ");
                    x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).SemiBold();
                });
            });
        }).GeneratePdf(filePath);
        _logger.LogInformation("PDF generated successfully: {FilePath}", filePath);
        return filePath;
    }
}