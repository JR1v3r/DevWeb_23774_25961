using System.Diagnostics;
using DevWeb_23774_25961.Data;
using Microsoft.AspNetCore.Mvc;
using DevWeb_23774_25961.Models;
using Microsoft.EntityFrameworkCore;

namespace DevWeb_23774_25961.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var trades = await _context.Trocas
            .Include(t => t.LivroDado)
            .Include(t => t.Vendedor)
            .Where(t => t.LivroDado != null && t.Vendedor != null)
            .ToListAsync();

        return View(trades); 
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