using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult RegisterAccount()
        {
            return View(); // Register.cshtml
        }


        public IActionResult LoginAccount()
        {
            return View(); // Login.cshtml
        }
    }
}
