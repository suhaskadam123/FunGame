using DataAccessLayer.Models;
using game_admin.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace game_admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public AccountController(AuthService authService, JwtService jwtService, JwtSettings jwtSettings)
        {
            _authService = authService;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings;
        }
       
        [HttpGet]
        public IActionResult LoginSuperAdmin()
        {
            return View("~/Views/Account/LoginSuperAdmin.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> LoginSuperAdmin(LoginModel model)
        {
            var user = await _authService.AuthenticateSuperAdmin(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Dashboard", "SuperAdmin");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginSuperAdmin", "Account");
        }
    }
}
