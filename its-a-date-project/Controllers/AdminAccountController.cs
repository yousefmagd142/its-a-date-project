using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using its_a_date_project.Data;
using its_a_date_project.Models.ViewModels;
using its_a_date_project.Services;

namespace its_a_date_project.Controllers
{
    public class AdminAccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly LoginRateLimiter _rateLimiter;
        private readonly PasswordHasher<string> _hasher = new();

        public AdminAccountController(AppDbContext db, LoginRateLimiter rateLimiter)
        {
            _db = db;
            _rateLimiter = rateLimiter;
        }

        [HttpGet("/admin/login")]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost("/admin/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var rateKey = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (_rateLimiter.IsLockedOut(rateKey))
            {
                model.Error = "Too many failed attempts. Try again in a few minutes.";
                return View(model);
            }

            var setting = _db.AdminSettings.FirstOrDefault();
            var ok = setting is not null &&
                     _hasher.VerifyHashedPassword("admin", setting.PasswordHash, model.Password) != PasswordVerificationResult.Failed;

            if (!ok)
            {
                _rateLimiter.RegisterFailure(rateKey);
                model.Error = "Wrong password.";
                return View(model);
            }
            _rateLimiter.RegisterSuccess(rateKey);

            var claims = new List<Claim> { new(ClaimTypes.Name, "admin") };
            var identity = new ClaimsIdentity(claims, "AdminAuth");
            await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(identity), new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
            });

            return RedirectToAction("Index", "AdminInvites");
        }

        [HttpPost("/admin/logout")]
        [Authorize(AuthenticationSchemes = "AdminAuth")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/admin/password")]
        [Authorize(AuthenticationSchemes = "AdminAuth")]
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        [HttpPost("/admin/password")]
        [Authorize(AuthenticationSchemes = "AdminAuth")]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var setting = _db.AdminSettings.First();

            if (_hasher.VerifyHashedPassword("admin", setting.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed)
            {
                model.Error = "Current password is wrong.";
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                model.Error = "New password and confirmation don't match.";
                return View(model);
            }

            setting.PasswordHash = _hasher.HashPassword("admin", model.NewPassword);
            _db.SaveChanges();

            return View(new ChangePasswordViewModel { Success = "Password updated." });
        }
    }
}
