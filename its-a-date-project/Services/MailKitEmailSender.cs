using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace its_a_date_project.Services
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(IOptions<SmtpOptions> options, ILogger<MailKitEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> SendDateNotificationAsync(string recipientEmail, string inviteSlug, DateTime chosenDateUtc, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Host))
            {
                _logger.LogWarning("Smtp:Host is not configured — skipping email for invite '{Slug}'.", inviteSlug);
                return false;
            }

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Invite '{Slug}' has no recipient email configured — skipping.", inviteSlug);
                return false;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(string.IsNullOrWhiteSpace(_options.From) ? _options.Username : _options.From));
                message.To.Add(MailboxAddress.Parse(recipientEmail));
                message.Subject = "She picked a date! 💗";
                message.Body = new TextPart("plain")
                {
                    Text = $"the most beautiful person said yes \"{inviteSlug}\" and picked:\n\n" +
                           $"{chosenDateUtc:dddd, MMMM d, yyyy} at {chosenDateUtc:h:mm tt}\n\n" +
                           "Sent automatically by Its A Date."
                };

                using var client = new SmtpClient();
                var socketOptions = _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                await client.ConnectAsync(_options.Host, _options.Port, socketOptions, ct);
                if (!string.IsNullOrWhiteSpace(_options.Username))
                    await client.AuthenticateAsync(_options.Username, _options.Password, ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send date notification email for invite '{Slug}'.", inviteSlug);
                return false;
            }
        }
    }
}
