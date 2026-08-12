namespace its_a_date_project.Services
{
    public interface IEmailSender
    {
        /// <summary>Sends the "she picked a date" notification. Returns false (and logs) instead of throwing
        /// when SMTP isn't configured yet, so the public submit flow never breaks because of email setup.</summary>
        Task<bool> SendDateNotificationAsync(string recipientEmail, string inviteSlug, DateTime chosenDateUtc, CancellationToken ct = default);
    }
}
