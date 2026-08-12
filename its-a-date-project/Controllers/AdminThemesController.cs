using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using its_a_date_project.Data;
using its_a_date_project.Models;
using its_a_date_project.Models.ViewModels;

namespace its_a_date_project.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    [Route("admin/themes")]
    public class AdminThemesController : Controller
    {
        private readonly AppDbContext _db;

        public AdminThemesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var themes = await _db.Themes
                .Select(t => new { Theme = t, InviteCount = t.Invites.Count })
                .OrderBy(x => x.Theme.Name)
                .ToListAsync();
            ViewData["InviteCounts"] = themes.ToDictionary(x => x.Theme.Id, x => x.InviteCount);
            return View(themes.Select(x => x.Theme).ToList());
        }

        [HttpGet("create")]
        public IActionResult Create() => View(new ThemeFormViewModel());

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemeFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var theme = ToEntity(model, new Theme());
            _db.Themes.Add(theme);
            await _db.SaveChangesAsync();

            TempData["Flash"] = $"Theme \"{theme.Name}\" created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var theme = await _db.Themes.FindAsync(id);
            if (theme is null) return NotFound();

            var model = new ThemeFormViewModel
            {
                Id = theme.Id,
                Name = theme.Name,
                BgStart = theme.BgStart,
                BgEnd = theme.BgEnd,
                CardBg = theme.CardBg,
                Ink = theme.Ink,
                InkSoft = theme.InkSoft,
                Accent = theme.Accent,
                AccentDeep = theme.AccentDeep,
                AccentSoft = theme.AccentSoft,
                Gold = theme.Gold,
                Border = theme.Border,
                ShadowRgba = theme.ShadowRgba,
            };
            return View(model);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThemeFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var theme = await _db.Themes.FindAsync(id);
            if (theme is null) return NotFound();

            ToEntity(model, theme);
            await _db.SaveChangesAsync();

            TempData["Flash"] = $"Theme \"{theme.Name}\" updated — every invite using it reflects this live.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var theme = await _db.Themes.Include(t => t.Invites).FirstOrDefaultAsync(t => t.Id == id);
            if (theme is null) return NotFound();

            if (theme.Invites.Count > 0)
            {
                TempData["Flash"] = $"Can't delete \"{theme.Name}\" — {theme.Invites.Count} invite(s) still use it.";
                return RedirectToAction(nameof(Index));
            }

            _db.Themes.Remove(theme);
            await _db.SaveChangesAsync();
            TempData["Flash"] = $"Theme \"{theme.Name}\" deleted.";
            return RedirectToAction(nameof(Index));
        }

        private static Theme ToEntity(ThemeFormViewModel model, Theme theme)
        {
            theme.Name = model.Name;
            theme.BgStart = model.BgStart;
            theme.BgEnd = model.BgEnd;
            theme.CardBg = model.CardBg;
            theme.Ink = model.Ink;
            theme.InkSoft = model.InkSoft;
            theme.Accent = model.Accent;
            theme.AccentDeep = model.AccentDeep;
            theme.AccentSoft = model.AccentSoft;
            theme.Gold = model.Gold;
            theme.Border = model.Border;
            theme.ShadowRgba = model.ShadowRgba;
            return theme;
        }
    }
}
