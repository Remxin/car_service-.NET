using EmailService.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailService.Services;
public class EmailSenderService(string senderEmail, string appPassword, BlobStorageService blobStorageService, ILogger<EmailSenderService> logger) {
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly BlobStorageService _blobStorageService = blobStorageService;
    private readonly string _senderEmail = senderEmail;
    private readonly string _appPassword = appPassword;

    public async Task SendWelcomeEmailAsync(string recipientEmail, string recipientName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Car Service", _senderEmail));
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));
        message.Subject = "👋 Welcome to Car Service!";

        var bodyBuilder = new BodyBuilder {
            HtmlBody = $@"
                <div style=""font-family: Arial, sans-serif; color: #333;"">
                    <h2 style=""color: #007bff;"">Hi {recipientName},</h2>
                    <p>We're excited to welcome you to <strong>Car Service</strong>!</p>
                    <p>
                        You now have access to our platform!
                    </p>
                    <hr style=""margin: 20px 0;"" />
                    <p style=""font-size: 0.9em;"">
                        If you have any questions, feel free to contact administrators
                    </p>
                    <p>Best regards,<br/>The Car Service Team 🚗</p>
                </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        await SendAsync(message, recipientEmail);
    }

    public async Task SendReportEmailAsync(List<Receiver> recipients, string reportPath, string messageText)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Car Service", _senderEmail));

        foreach (var recipient in recipients) {
            message.To.Add(new MailboxAddress(recipient.Name, recipient.Email));
        }

        message.Subject = "📄 Your Car Service Report";

        var bodyBuilder = new BodyBuilder {
            HtmlBody = $@"
                <div style=""font-family: Arial, sans-serif; color: #333;"">
                    <h2 style=""color: #007bff;"">Hello,</h2>
                    <p>{messageText}</p>
                    <p>We've attached the detailed report as a PDF file for your reference.</p>
                    <p>Thank you for using Car Service!</p>
                    <hr style=""margin: 20px 0;"" />
                    <p style=""font-size: 0.9em;"">
                        For any inquiries, contact administrators
                    </p>
                    <p>Kind regards,<br/>The Car Service Team 🚗</p>
                </div>"
        };

        var reportStream = await _blobStorageService.GetFileStreamAsync(reportPath);
        if (reportStream == null) {
            logger.LogWarning("⚠️ Report stream is null for path: {Path}", reportPath);
            return;
        }
        bodyBuilder.Attachments.Add("report.pdf", reportStream, new ContentType("application", "pdf"));

        message.Body = bodyBuilder.ToMessageBody();

        await SendAsync(message, string.Join(", ", recipients.Select(r => r.Email)));
    }

    private async Task SendAsync(MimeMessage message, string contextInfo) {
        try {
            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_senderEmail, _appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("✅ Email sent to: {Recipients}", contextInfo);
        }
        catch (Exception ex) {
            logger.LogError(ex, "❌ Failed to send email to: {Recipients}", contextInfo);
        }
    }
}