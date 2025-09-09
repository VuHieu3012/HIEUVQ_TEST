using Microsoft.AspNetCore.Mvc;
using AuthModule.Models;

namespace AuthModule.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
