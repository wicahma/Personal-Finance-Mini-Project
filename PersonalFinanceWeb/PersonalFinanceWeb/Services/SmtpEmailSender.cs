using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using PersonalFinance.Infrastructure.Identity;

namespace PersonalFinanceWeb.Services;

public sealed class SmtpSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromName { get; init; } = "Personal Finance";
    public string FromEmail { get; init; } = string.Empty;
    public bool UseSsl { get; init; } = false;
}

public sealed class SmtpEmailSender(
    IOptions<SmtpSettings> options,
    ILogger<SmtpEmailSender> logger) : IEmailSender<ApplicationUser>
{
    private readonly SmtpSettings _settings = options.Value;

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendEmailAsync(
            email,
            "Confirm your email address",
            BuildHtmlEmail(
                heading: "Confirm Your Email",
                accentColor: "#a3e635",
                bodyHtml: $"""
                    <p style="margin:0 0 24px;font-size:16px;line-height:1.6;">
                        Welcome! Please confirm your email address by clicking the button below.
                    </p>
                    <a href="{confirmationLink}"
                       style="display:inline-block;background:#a3e635;color:#000;font-weight:700;
                              font-size:15px;padding:12px 28px;text-decoration:none;
                              border:2px solid #000;box-shadow:4px 4px 0 #000;">
                        Confirm Email
                    </a>
                    <p style="margin:24px 0 0;font-size:13px;color:#aaa;">
                        If you did not create an account, you can safely ignore this email.
                    </p>
                    """));

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(
            email,
            "Reset your password",
            BuildHtmlEmail(
                heading: "Reset Your Password",
                accentColor: "#22d3ee",
                bodyHtml: $"""
                    <p style="margin:0 0 24px;font-size:16px;line-height:1.6;">
                        We received a request to reset the password for your account.
                        Click the button below to choose a new password.
                    </p>
                    <a href="{resetLink}"
                       style="display:inline-block;background:#22d3ee;color:#000;font-weight:700;
                              font-size:15px;padding:12px 28px;text-decoration:none;
                              border:2px solid #000;box-shadow:4px 4px 0 #000;">
                        Reset Password
                    </a>
                    <p style="margin:24px 0 0;font-size:13px;color:#aaa;">
                        This link will expire in 24 hours. If you did not request a password reset,
                        please ignore this email.
                    </p>
                    """));

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(
            email,
            "Your password reset code",
            BuildHtmlEmail(
                heading: "Password Reset Code",
                accentColor: "#f97316",
                bodyHtml: $"""
                    <p style="margin:0 0 16px;font-size:16px;line-height:1.6;">
                        Use the following code to reset your password:
                    </p>
                    <div style="display:inline-block;background:#111;border:2px solid #f97316;
                                box-shadow:4px 4px 0 #f97316;padding:16px 32px;
                                font-size:28px;font-weight:700;letter-spacing:8px;
                                color:#f97316;font-family:monospace;">
                        {resetCode}
                    </div>
                    <p style="margin:24px 0 0;font-size:13px;color:#aaa;">
                        This code expires in 10 minutes. If you did not request this, ignore this email.
                    </p>
                    """));


    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();

            var secureSocketOptions = _settings.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(_settings.Host, _settings.Port, secureSocketOptions);

            if (!string.IsNullOrEmpty(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);

            logger.LogInformation("Email sent to {Email} — subject: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email} — subject: {Subject}", toEmail, subject);
            throw;
        }
    }


    private static string BuildHtmlEmail(string heading, string accentColor, string bodyHtml) => $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="UTF-8"/>
          <meta name="viewport" content="width=device-width,initial-scale=1"/>
          <title>{heading}</title>
        </head>
        <body style="margin:0;padding:0;background:#0a0a0a;font-family:'Courier New',Courier,monospace;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#0a0a0a;padding:40px 0;">
            <tr>
              <td align="center">
                <table role="presentation" width="520" cellpadding="0" cellspacing="0"
                       style="background:#0a0a0a;border:2px solid {accentColor};
                              box-shadow:6px 6px 0 {accentColor};padding:40px;">
                  <tr>
                    <td>
                      <div style="border-bottom:2px solid {accentColor};padding-bottom:20px;margin-bottom:28px;">
                        <span style="font-size:11px;font-weight:700;letter-spacing:3px;
                                     color:{accentColor};text-transform:uppercase;">
                          Personal Finance
                        </span>
                        <h1 style="margin:8px 0 0;font-size:22px;font-weight:700;
                                   color:#fff;letter-spacing:1px;">
                          {heading}
                        </h1>
                      </div>
                      <div style="color:#e5e5e5;">
                        {bodyHtml}
                      </div>
                    </td>
                  </tr>
                </table>
                <p style="margin:16px 0 0;font-size:11px;color:#555;">
                  © {DateTime.UtcNow.Year} Personal Finance. All rights reserved.
                </p>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
}
