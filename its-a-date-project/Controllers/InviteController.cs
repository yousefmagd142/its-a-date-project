using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using its_a_date_project.Data;
using its_a_date_project.Models;
using its_a_date_project.Models.ViewModels;
using its_a_date_project.Services;

namespace its_a_date_project.Controllers
{
    public class InviteController : Controller
    {
        private const string LangCookie = "lang";

        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;

        public InviteController(AppDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index(string? lang)
        {
            var invite = await _db.Invites
                .Include(i => i.Theme)
                .FirstOrDefaultAsync(i => i.IsDefault && i.IsActive);

            if (invite is null)
                return View("NoInvite");

            return await RenderInvite(invite, lang);
        }

        [HttpGet("/i/{slug}")]
        public async Task<IActionResult> Show(string slug, string? lang)
        {
            var invite = await _db.Invites
                .Include(i => i.Theme)
                .FirstOrDefaultAsync(i => i.Slug == slug && i.IsActive);

            if (invite is null)
                return NotFound();

            return await RenderInvite(invite, lang);
        }

        [HttpPost("/i/{slug}/submit")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Submit(string slug, [FromBody] SubmitRequest request, CancellationToken ct)
        {
            var invite = await _db.Invites.FirstOrDefaultAsync(i => i.Slug == slug && i.IsActive, ct);
            if (invite is null)
                return NotFound();

            if (request is null || !DateTime.TryParseExact(
                    $"{request.Date}T{request.Time}", "yyyy-MM-ddTHH:mm",
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var chosen))
                return BadRequest();

            var submission = new DateSubmission
            {
                InviteId = invite.Id,
                ChosenDateUtc = chosen,
            };
            _db.DateSubmissions.Add(submission);
            await _db.SaveChangesAsync(ct);

            var sent = await _emailSender.SendDateNotificationAsync(invite.RecipientEmail, invite.Slug, chosen, ct);
            if (sent)
            {
                submission.EmailSent = true;
                await _db.SaveChangesAsync(ct);
            }

            return Ok(new { success = true, emailSent = sent });
        }

        private async Task<IActionResult> RenderInvite(Invite invite, string? langQuery)
        {
            var cookieLang = Request.Cookies.TryGetValue(LangCookie, out var c) ? c : null;
            var lang = ResolveLanguage(invite, langQuery, cookieLang);
            if (langQuery is "en" or "ar")
            {
                Response.Cookies.Append(LangCookie, lang, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                });
            }

            var theme = invite.Theme ?? await _db.Themes.FindAsync(invite.ThemeId);
            var text = await _db.InviteTexts
                .Where(t => t.InviteId == invite.Id && t.Language == lang)
                .ToDictionaryAsync(t => t.Key, t => t.Value);

            // Fall back to English for any key missing in the requested language (keeps the page from breaking
            // if an admin edits English but hasn't filled in Arabic for a brand-new key yet).
            if (lang != "en")
            {
                var english = await _db.InviteTexts
                    .Where(t => t.InviteId == invite.Id && t.Language == "en")
                    .ToDictionaryAsync(t => t.Key, t => t.Value);
                foreach (var key in TextKeys.All)
                    if (!text.ContainsKey(key) && english.TryGetValue(key, out var fallback))
                        text[key] = fallback;
            }

            var vm = new InviteViewModel
            {
                Invite = invite,
                Theme = theme!,
                Text = text,
                Lang = lang,
            };
            return View("Show", vm);
        }

        private static string ResolveLanguage(Invite invite, string? langQuery, string? cookieLang)
        {
            if (langQuery is "en" or "ar") return langQuery;
            if (cookieLang is "en" or "ar") return cookieLang;
            return invite.DefaultLanguage is "en" or "ar" ? invite.DefaultLanguage : "en";
        }

        public class SubmitRequest
        {
            public string Date { get; set; } = "";
            public string Time { get; set; } = "";
        }
    }
}
