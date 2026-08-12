using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using its_a_date_project.Data;

namespace its_a_date_project.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    [Route("admin/submissions")]
    public class AdminSubmissionsController : Controller
    {
        private readonly AppDbContext _db;

        public AdminSubmissionsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? inviteId)
        {
            var query = _db.DateSubmissions.Include(s => s.Invite).OrderByDescending(s => s.SubmittedAtUtc).AsQueryable();
            if (inviteId is not null) query = query.Where(s => s.InviteId == inviteId);

            ViewData["Invites"] = await _db.Invites.OrderBy(i => i.Slug).ToListAsync();
            ViewData["SelectedInviteId"] = inviteId;
            return View(await query.ToListAsync());
        }
    }
}
