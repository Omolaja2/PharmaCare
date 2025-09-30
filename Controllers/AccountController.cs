using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Data;
using PharmacyApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PharmacyApp.Services;

public class AccountController : Controller
{
    private readonly PharmacyDbContext _context;
    private readonly EmailService _emailService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(PharmacyDbContext context, EmailService emailService, ILogger<AccountController> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
        _logger.LogInformation("registering atempt for {Email}", user.Email);
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("registration failed !: {Email} already exist", user.Email);
                ViewBag.Error = "Email already registered. Please log in instead!";
                return View(user);
            }

            if (string.IsNullOrEmpty(user.Role))
                user.Role = "Patient";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New user {FullName} registered with role {Role}", user.FullName, user.Role);
            try
            {
                await _emailService.SendEmailAsync(user.Email,
                    "Welcome to PharmaCare!",
                    $"<h1>Welcome {user.FullName}!</h1><p>You registered as {user.Role}.</p>");
                _logger.LogInformation("Welcome email sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
            }

            return RedirectToAction("Login");
        }

        _logger.LogWarning("registration failed for {Email}: invalid model state", user.Email);
        return View(user);
    }

    public IActionResult Login() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        _logger.LogInformation("Login attempt for {Email}", email);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true }
            );

            _logger.LogInformation("user {Email} logged in successfully with role {Role}", user.Email, user.Role);

            if (user.Role == "Pharmacist")
                return RedirectToAction("MyPharmacy", "Pharmacy");
            else
                return RedirectToAction("Dashboard", "Order");
        }

        _logger.LogWarning("Invalid login attempt for {Email}", email);
        ViewBag.Error = "Invalid email or password";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User {Email} logged out", userEmail ?? "Unknown");
        return RedirectToAction("Login");
    }

    public IActionResult Profile()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        _logger.LogInformation("Profile page accessed by {Email}", userEmail ?? "unknown");
        return View();
    }
}
