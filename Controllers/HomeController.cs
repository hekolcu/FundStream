using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FundStream.Models;
using Microsoft.AspNetCore.Identity;
namespace FundStream.Controllers;

public class HomeController : Controller
{
    private readonly FundStreamContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, FundStreamContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // Check if the user is already logged in
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            // If logged in, redirect to /projects
            return RedirectToAction("Index", "Projects");
        }
        // If not logged in, show the Auth view
        return RedirectToAction("Auth");
    }

    [HttpGet("Auth")]
    public IActionResult Auth()
    {
        // If user is already logged in, redirect to /projects
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Projects");
        }
        // Otherwise, show the Auth view
        return View();
    }

    [HttpPost("Register")]
    public IActionResult Register(User userForm)
    {
        if (ModelState.IsValid)
        {
            // Hash the password
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            userForm.Password = hasher.HashPassword(userForm, userForm.Password);

            // Add the user to the database
            _context.Users.Add(userForm);
            _context.SaveChanges();

            // Here, you can set the user's ID in the session
            HttpContext.Session.SetInt32("UserId", userForm.UserId);

            return RedirectToAction("Index");
        }
        return View("Auth");
    }

    [HttpPost("LogIn")]
    public IActionResult LogIn(string email, string password)
    {
        var userInDb = _context.Users.FirstOrDefault(u => u.Email == email);

        if(userInDb != null)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(userInDb, userInDb.Password, password);
            if(result == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Index");
            }
        }

        ModelState.AddModelError("LoginFailed", "Invalid Login Attempt");
        return View("Auth");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}