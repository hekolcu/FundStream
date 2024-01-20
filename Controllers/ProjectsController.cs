using FundStream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FundStream.Controllers;

public class ProjectsController : Controller
{
    private readonly FundStreamContext _context;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(ILogger<ProjectsController> logger, FundStreamContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // Ensure the user is logged in before showing them the projects
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Auth", "Home");
        }
        
        var projects = _context.Projects
            .Include(p => p.Creator)
            .ToList();
        var contributions = _context.Contributions.ToList();
        
        ViewBag.LoggedInUserId = userId;
        ViewBag.IsUserLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
        ViewBag.ProjectCount = projects.Count;
        ViewBag.TotalAmountFunded = projects.Sum(project => project.AmountRaised);
        ViewBag.ContributionCount = contributions.Count;
        return View(projects);
    }

    // GET: Projects/Details/5
    public IActionResult Details(int? id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Auth", "Home");
        }

        var project = _context.Projects
            .Include(p => p.Creator)
            .Include(p => p.Contributions)
            .FirstOrDefault(m => m.ProjectId == id);
    
        if (project == null)
        {
            return NotFound();
        }
        
        ViewBag.LoggedInUserId = userId;
        ViewBag.IsUserLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
        var hasContributed = _context.Contributions.Any(c => c.UserId == userId && c.ProjectId == project.ProjectId);
        ViewBag.HasContributed = hasContributed;
        _logger.Log(LogLevel.Information, "Has Contributed: " + (hasContributed ? "Yes" : "No"));
        return View(project);
    }
    
    // GET: Projects/Create
    public IActionResult Create()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Auth", "Home");
        }

        return View();
    }

    // POST: Projects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Project project)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Auth", "Home");
        }

        if (ModelState.IsValid)
        {
            project.UserId = userId.Value;
            _context.Projects.Add(project);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }
    
    // GET: Projects/Delete/5
    public IActionResult Delete(int? id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Auth", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var project = _context.Projects.FirstOrDefault(m => m.ProjectId == id);
    
        if (project == null)
        {
            return NotFound();
        }

        if (project.UserId != HttpContext.Session.GetInt32("UserId"))
        {
            return Unauthorized();
        }

        return View(project);
    }

    // POST: Projects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var project = _context.Projects.Find(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
    
    // POST: Projects/Support
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Support(int projectId, decimal supportAmount)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || supportAmount <= 0)
        {
            return RedirectToAction("Auth", "Home");
        }

        var project = _context.Projects.Find(projectId);
        if (project == null)
        {
            return NotFound();
        }

        // Create a new contribution record
        var contribution = new Contribution
        {
            ProjectId = projectId,
            UserId = userId.Value,
            Amount = supportAmount,
            CreatedAt = DateTime.Now
        };

        // Save the project
        project.AmountRaised += supportAmount;
        _context.Projects.Update(project);
        _context.SaveChanges();

        // Save the contribution
        _context.Contributions.Add(contribution);
        _context.SaveChanges();

        // Redirect back to the project details
        return RedirectToAction("Details", new { id = projectId });
    }
}