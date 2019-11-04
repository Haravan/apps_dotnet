using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

        public IActionResult Index()
        {
            var userName = User.Claims.FirstOrDefault(m => m.Type == "name").Value;
            return View();
        }

        [Route("/request_app_accestoken_callback")]
        public async Task<IActionResult> request_app_accestoken_callback() 
        {
            var result = await HttpContext.AuthenticateAsync(HaravanAuthenticationConsts.ServiceScheme);
            return Redirect("/");
        }

        [Route("/request_app_accestoken")]
        public IActionResult request_app_accestoken() 
        {
            return Challenge(new AuthenticationProperties() {
                RedirectUri = "https://localhost:5001/request_app_accestoken_callback"
            }, HaravanAuthenticationConsts.ServiceScheme);
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
