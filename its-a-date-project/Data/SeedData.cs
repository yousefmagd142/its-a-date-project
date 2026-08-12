using Microsoft.AspNetCore.Identity;
using its_a_date_project.Models;

namespace its_a_date_project.Data
{
    public static class SeedData
    {
        // English strings copied verbatim from the original static page. Also reused as the starting
        // point when an admin creates a brand-new invite.
        public static readonly Dictionary<string, string> EnglishText = new()
        {
            ["Eyebrow"] = "a very important question",
            ["Headline"] = "Will you go on a date with me?",
            ["Subhead"] = "Take your time. Well — not too much time.",
            ["YesButton"] = "Yes 💗",
            ["NoButton"] = "No",
            ["DodgeLine1"] = "huh, missed me",
            ["DodgeLine2"] = "is that a yes?",
            ["DodgeLine3"] = "come on now 🙂",
            ["DodgeLine4"] = "you can't catch this button",
            ["DodgeLine5"] = "pretty please?",
            ["DodgeLine6"] = "I'll just wait here 💗",
            ["PlanEyebrow"] = "yay!! 💌",
            ["PlanHeadline"] = "Let's pick the day",
            ["PlanSubhead"] = "Choose whenever suits you best.",
            ["DateLabel"] = "Date",
            ["TimeLabel"] = "Time",
            ["SubmitButton"] = "Lock it in",
            ["ThanksEyebrow"] = "it's a date 💕",
            ["ThanksHeadline"] = "Thank you!",
            ["ThanksMessageTemplate"] = "I can't wait to see you on {date} at {time}.",
            ["LoveLine"] = "I love you so much.",
        };

        // Modern Standard Arabic translation (not machine-literal), feminine address, same emoji.
        public static readonly Dictionary<string, string> ArabicText = new()
        {
            ["Eyebrow"] = "سؤال في غاية الأهمية",
            ["Headline"] = "هل توافقين على الخروج في موعد معي؟",
            ["Subhead"] = "خذي وقتك... لكن ليس وقتًا طويلًا جدًا",
            ["YesButton"] = "نعم 💗",
            ["NoButton"] = "لا",
            ["DodgeLine1"] = "ها، فاتك",
            ["DodgeLine2"] = "هل هذا يعني نعم؟",
            ["DodgeLine3"] = "هيا الآن 🙂",
            ["DodgeLine4"] = "لن تستطيعي الإمساك بهذا الزر",
            ["DodgeLine5"] = "أرجوكِ",
            ["DodgeLine6"] = "سأنتظر هنا فقط 💗",
            ["PlanEyebrow"] = "ياي!! 💌",
            ["PlanHeadline"] = "لنحدد اليوم",
            ["PlanSubhead"] = "اختاري الوقت الذي يناسبك",
            ["DateLabel"] = "التاريخ",
            ["TimeLabel"] = "الوقت",
            ["SubmitButton"] = "تأكيد الموعد",
            ["ThanksEyebrow"] = "لقد تحدد الموعد 💕",
            ["ThanksHeadline"] = "شكرًا لكِ!",
            ["ThanksMessageTemplate"] = "لا أطيق الانتظار لرؤيتكِ يوم {date} الساعة {time}.",
            ["LoveLine"] = "أحبكِ كثيرًا.",
        };

        /// <summary>Seeds the Pink theme, the default "welcome" invite, its EN/AR text, and the admin password.
        /// Returns the generated admin password when a new one was created, otherwise null.</summary>
        public static string? EnsureSeeded(AppDbContext db)
        {
            string? generatedPassword = null;

            var pink = db.Themes.FirstOrDefault(t => t.Name == "Pink");
            if (pink is null)
            {
                pink = new Theme
                {
                    Name = "Pink",
                    BgStart = "#ffeaf4",
                    BgEnd = "#ffd2e6",
                    CardBg = "#fffbfd",
                    Ink = "#5c1f3d",
                    InkSoft = "#93486e",
                    Accent = "#ff5c9e",
                    AccentDeep = "#e0227a",
                    AccentSoft = "#ffd3e7",
                    Gold = "#ffb84d",
                    Border = "#ffcfe4",
                    ShadowRgba = "rgba(224,34,122,0.18)",
                };
                db.Themes.Add(pink);
                db.SaveChanges();
            }

            var welcome = db.Invites.FirstOrDefault(i => i.Slug == "welcome");
            if (welcome is null)
            {
                welcome = new Invite
                {
                    Slug = "welcome",
                    ThemeId = pink.Id,
                    RecipientEmail = "",
                    DefaultLanguage = "en",
                    IsDefault = true,
                    IsActive = true,
                };
                db.Invites.Add(welcome);
                db.SaveChanges();
            }

            if (!db.InviteTexts.Any(t => t.InviteId == welcome.Id))
            {
                SeedDefaultTextFor(db, welcome.Id);
            }

            if (!db.AdminSettings.Any())
            {
                generatedPassword = GenerateRandomPassword();
                var hasher = new PasswordHasher<string>();
                var hash = hasher.HashPassword("admin", generatedPassword);
                db.AdminSettings.Add(new AdminSetting { PasswordHash = hash });
                db.SaveChanges();
            }

            return generatedPassword;
        }

        /// <summary>Seeds the default English + Arabic text (21 keys) for an invite that doesn't have any yet —
        /// used both for the initial "welcome" invite and as the starting point for newly created ones.</summary>
        public static void SeedDefaultTextFor(AppDbContext db, int inviteId)
        {
            foreach (var key in TextKeys.All)
            {
                db.InviteTexts.Add(new InviteText { InviteId = inviteId, Language = "en", Key = key, Value = EnglishText[key] });
                db.InviteTexts.Add(new InviteText { InviteId = inviteId, Language = "ar", Key = key, Value = ArabicText[key] });
            }
            db.SaveChanges();
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            var bytes = new byte[16];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            var sb = new System.Text.StringBuilder();
            foreach (var b in bytes)
                sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }
    }
}
