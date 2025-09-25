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

    public AccountController(PharmacyDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered. Please log in instead.";
                return View(user);
            }

            if (string.IsNullOrEmpty(user.Role))
                user.Role = "Patient"; 

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Send welcome email ASAP
            await _emailService.SendEmailAsync(user.Email, 
                " Welcome to PharmaCare!", 
                $"<h1>Welcome {user.FullName}!</h1><p>You registered as {user.Role}.</p>");

            return RedirectToAction("Login");
        }
        return View(user);
    }

    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
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

            if (user.Role == "Pharmacist")
                return RedirectToAction("MyPharmacy", "Pharmacy");
            else
                return RedirectToAction("Dashboard", "Order");
        }

        ViewBag.Error = "Invalid email or password";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult Profile()
    {
        return View();
    }
}
