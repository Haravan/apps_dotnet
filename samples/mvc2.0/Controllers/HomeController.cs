using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models;

namespace mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("/login_callback")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback() 
        {
            var result = await HttpContext.AuthenticateAsync("Haravan");
            await HttpContext.SignInAsync(result.Principal, result.Properties);
            return Redirect("/");
        }

        [Route("/login")]
        [AllowAnonymous]
        public IActionResult Login() 
        {
            return Challenge(new AuthenticationProperties() {
                RedirectUri = "https://localhost:5001/login_callback"
            }, "Haravan");
        }

        public IActionResult Index()
        {
            var userName = User.Claims.FirstOrDefault(m => m.Type == "name").Value;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
