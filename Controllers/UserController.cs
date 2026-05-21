using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RosATest.Model.Entity;
using RosATest.ViewModels;

namespace RosATest.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("List", "InquiryRequests");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user;

            try
            {
                user = await _userService.IdentifyAsync(model.Email);
            } catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            
            try
            {
                // Источник формулировок ошибок - usecases (services)
                _userService.AuthenticateAsync(user, model.Password);
            } catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            try {
                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var session = await _userService.CreateSessionAsync(user, ipAddress, userAgent);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    // TODO: вынести магические значения в конcтанты
                    new Claim("GroupCodename", user.Group.Codename.ToString()),
                    new Claim("SessionToken", session.SessionToken)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                    ),
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в процессе входа");
                ModelState.AddModelError(string.Empty, "Произошла ошибка. Попробуйте позже.");
                return View();
            }
            
            return RedirectToAction("List", "InquiryRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // TODO: вынести магические значения в конcтанты
            string? sessionToken = User.FindFirstValue("SessionToken");

            if(sessionToken != null)
            {
                await _userService.LogoutAsync(sessionToken);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            Response.Cookies.Delete("AuthCookie");

            return RedirectToAction("Login");
        }
    }
}