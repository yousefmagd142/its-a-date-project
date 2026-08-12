using its_a_date_project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace its_a_date_project.Controllers
{
    // Only the error handler remains — InviteController owns "/" and every /i/{slug} page now.
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
