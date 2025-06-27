using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWeb_23774_25961.Controllers;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }
    //GET
    [HttpGet]
    public async Task<IActionResult> Index(String query)
    {
        //ViewBag.CurrentSearch = query;
        if (string.IsNullOrEmpty(query))
        {
            return View(new List<Livros>());
        }

        var resultado = await _context.Livros
            .Where(l => l.Titulo.Contains(query) || l.Autor.Contains(query) || l.ISBN.Contains(query)).Take(20)
            .ToListAsync();
        
        return View(resultado);
    }
}