using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace SudoBox.UnifiedModule.Application.Users.Utils.Email;

public sealed class SmtpEmailSender(IConfiguration cfg) : IEmailSender
{
    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        var host = cfg["Email:Smtp:Host"] ?? "localhost";
        var port = int.TryParse(cfg["Email:Smtp:Port"], out var p) ? p : 25;
        var user = cfg["Email:Smtp:User"];
        var pass = cfg["Email:Smtp:Pass"];
        var from = cfg["Email:From"] ?? "no-reply@sudobox.local";

        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress("SudoBox", from));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = subject;
        msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable, ct);

        if (!string.IsNullOrWhiteSpace(user))
            await client.AuthenticateAsync(user, pass, ct);

        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }
}