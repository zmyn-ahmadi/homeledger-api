using System.Net;
using System.Net.Mail;

namespace HomeLedger.API.Services.Email;

public class SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    : IEmailService
{
    private readonly IConfiguration _config = config;
    private readonly ILogger<SmtpEmailService> _logger = logger;

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var host = _config["Smtp:Host"];

        // No SMTP configured (e.g. local dev) — log the email instead of failing
        // so the invitation flow stays testable end-to-end.
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogWarning(
                "Smtp:Host is not configured — email to {To} was not sent. Subject: {Subject}\n{Body}",
                to,
                subject,
                htmlBody
            );
            return;
        }

        var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
        var from =
            _config["Smtp:From"]
            ?? _config["Smtp:Username"]
            ?? throw new InvalidOperationException("Smtp:From is not configured");

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = !bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) || ssl,
            Credentials = new NetworkCredential(
                _config["Smtp:Username"],
                _config["Smtp:Password"]
            ),
        };

        using var message = new MailMessage(from, to, subject, htmlBody) { IsBodyHtml = true };

        await client.SendMailAsync(message);

        _logger.LogInformation("Sent email to {To} with subject {Subject}", to, subject);
    }
}
