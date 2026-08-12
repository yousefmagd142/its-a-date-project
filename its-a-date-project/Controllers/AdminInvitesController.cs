using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using its_a_date_project.Data;
using its_a_date_project.Models;
using its_a_date_project.Models.ViewModels;

namespace its_a_date_project.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class AdminInvitesController : Controller
    {
        private readonly AppDbContext _db;

        public AdminInvitesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("/admin")]
        [HttpGet("/admin/invites")]
        public async Task<IActionResult> Index()
        {
            var invites = await _db.Invites.Include(i => i.Theme).OrderBy(i => i.Slug).ToListAsync();
            return View(invites);
        }

        [HttpGet("/admin/invites/create")]
        public async Task<IActionResult> Create()
        {
            var model = new InviteFormViewModel { AvailableThemes = await _db.Themes.OrderBy(t => t.Name).ToListAsync() };
            return View(model);
        }

        [HttpPost("/admin/invites/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InviteFormViewModel model)
        {
            model.AvailableThemes = await _db.Themes.OrderBy(t => t.Name).ToListAsync();

            if (await _db.Invites.AnyAsync(i => i.Slug == model.Slug))
            {
                model.Error = $"Slug \"{model.Slug}\" is already used by another invite.";
                return View(model);
            }
            if (!ModelState.IsValid) return View(model);

            var invite = new Invite
            {
                Slug = model.Slug,
                ThemeId = model.ThemeId,
                RecipientEmail = model.RecipientEmail,
                DefaultLanguage = model.DefaultLanguage,
                IsActive = model.IsActive,
                IsDefault = false,
            };
            _db.Invites.Add(invite);
            await _db.SaveChangesAsync();

            SeedData.SeedDefaultTextFor(_db, invite.Id);

            TempData["Flash"] = $"Invite \"{invite.Slug}\" created at /i/{invite.Slug} — customize its text next.";
            return RedirectToAction(nameof(Text), new { id = invite.Id });
        }

        [HttpGet("/admin/invites/{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var invite = await _db.Invites.FindAsync(id);
            if (invite is null) return NotFound();

            var model = new InviteFormViewModel
            {
                Id = invite.Id,
                Slug = invite.Slug,
                ThemeId = invite.ThemeId,
                RecipientEmail = invite.RecipientEmail,
                DefaultLanguage = invite.DefaultLanguage,
                IsActive = invite.IsActive,
                AvailableThemes = await _db.Themes.OrderBy(t => t.Name).ToListAsync(),
            };
            return View(model);
        }

        [HttpPost("/admin/invites/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InviteFormViewModel model)
        {
            model.AvailableThemes = await _db.Themes.OrderBy(t => t.Name).ToListAsync();

            var invite = await _db.Invites.FindAsync(id);
            if (invite is null) return NotFound();

            if (await _db.Invites.AnyAsync(i => i.Slug == model.Slug && i.Id != id))
            {
                model.Error = $"Slug \"{model.Slug}\" is already used by another invite.";
                return View(model);
            }
            if (!ModelState.IsValid) return View(model);

            invite.Slug = model.Slug;
            invite.ThemeId = model.ThemeId;
            invite.RecipientEmail = model.RecipientEmail;
            invite.DefaultLanguage = model.DefaultLanguage;
            invite.IsActive = model.IsActive;
            await _db.SaveChangesAsync();

            TempData["Flash"] = $"Invite \"{invite.Slug}\" updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/admin/invites/{id:int}/set-default")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var invites = await _db.Invites.ToListAsync();
            foreach (var i in invites) i.IsDefault = i.Id == id;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/admin/invites/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var invite = await _db.Invites.FindAsync(id);
            if (invite is null) return NotFound();

            if (invite.IsDefault)
            {
                TempData["Flash"] = "Can't delete the default invite — set another one as default first.";
                return RedirectToAction(nameof(Index));
            }

            _db.Invites.Remove(invite);
            await _db.SaveChangesAsync();
            TempData["Flash"] = $"Invite \"{invite.Slug}\" deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/admin/invites/{id:int}/text")]
        public async Task<IActionResult> Text(int id)
        {
            var invite = await _db.Invites.FindAsync(id);
            if (invite is null) return NotFound();

            var model = await BuildTextEditViewModel(invite);
            return View(model);
        }

        [HttpPost("/admin/invites/{id:int}/text")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Text(int id, InviteTextEditViewModel posted)
        {
            var invite = await _db.Invites.FindAsync(id);
            if (invite is null) return NotFound();

            var existing = await _db.InviteTexts.Where(t => t.InviteId == id).ToListAsync();
            foreach (var field in posted.Fields)
            {
                var en = existing.FirstOrDefault(t => t.Language == "en" && t.Key == field.Key);
                if (en is null) _db.InviteTexts.Add(new InviteText { InviteId = id, Language = "en", Key = field.Key, Value = field.English });
                else en.Value = field.English;

                var ar = existing.FirstOrDefault(t => t.Language == "ar" && t.Key == field.Key);
                if (ar is null) _db.InviteTexts.Add(new InviteText { InviteId = id, Language = "ar", Key = field.Key, Value = field.Arabic });
                else ar.Value = field.Arabic;
            }
            await _db.SaveChangesAsync();

            var model = await BuildTextEditViewModel(invite);
            model.Success = "Text saved.";
            return View(model);
        }

        private async Task<InviteTextEditViewModel> BuildTextEditViewModel(Invite invite)
        {
            var texts = await _db.InviteTexts.Where(t => t.InviteId == invite.Id).ToListAsync();
            var model = new InviteTextEditViewModel { InviteId = invite.Id, Slug = invite.Slug };
            foreach (var key in TextKeys.All)
            {
                model.Fields.Add(new TextFieldViewModel
                {
                    Key = key,
                    English = texts.FirstOrDefault(t => t.Language == "en" && t.Key == key)?.Value ?? "",
                    Arabic = texts.FirstOrDefault(t => t.Language == "ar" && t.Key == key)?.Value ?? "",
                });
            }
            return model;
        }
    }
}
