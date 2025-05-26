using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Controllers
{
    public class TrocasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TrocasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trocas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Trocas.Include(t => t.Comprador).Include(t => t.LivroDado).Include(t => t.LivroRecebido).Include(t => t.Vendedor);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Livros/MyTrades
        public async Task<IActionResult> MyTrades()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var minhasTrocas = await _context.Trocas
                .Include(t => t.LivroDado)
                .Include(t => t.Vendedor)
                .Where(t => t.IdVendedor == user.Id)
                .ToListAsync();

            return View(minhasTrocas);
        }
        // GET: Trocas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trocas = await _context.Trocas
                .Include(t => t.Comprador)
                .Include(t => t.LivroDado)
                .Include(t => t.LivroRecebido)
                .Include(t => t.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trocas == null)
            {
                return NotFound();
            }

            return View(trocas);
        }

        // GET: Trocas/Create
        public IActionResult Create()
        {
            ViewData["IdComprador"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["IdLivroDado"] = new SelectList(_context.Livros, "Id", "Autor");
            ViewData["IdLivroRecebido"] = new SelectList(_context.Livros, "Id", "Autor");
            ViewData["IdVendedor"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Trocas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLivroDado")] Trocas trocas)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" }); // uwu
            }

            // Set remaining properties
            trocas.IdVendedor = user.Id;
            trocas.Estado = Trocas.EstadoTroca.Pendente;
            trocas.Timestamp = DateTime.Now;

            // Optional fields will stay null: IdComprador & IdLivroRecebido

            if (ModelState.IsValid)
            {
                _context.Add(trocas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Just in case validation fails... load ViewData again~
            ViewData["IdLivroDado"] = new SelectList(_context.Livros, "Id", "Titulo", trocas.IdLivroDado);
            return View(trocas);
        }


        // GET: Trocas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trocas = await _context.Trocas.FindAsync(id);
            if (trocas == null)
            {
                return NotFound();
            }
            ViewData["IdComprador"] = new SelectList(_context.Users, "Id", "Id", trocas.IdComprador);
            ViewData["IdLivroDado"] = new SelectList(_context.Livros, "Id", "Autor", trocas.IdLivroDado);
            ViewData["IdLivroRecebido"] = new SelectList(_context.Livros, "Id", "Autor", trocas.IdLivroRecebido);
            ViewData["IdVendedor"] = new SelectList(_context.Users, "Id", "Id", trocas.IdVendedor);
            return View(trocas);
        }

        // POST: Trocas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdLivroDado,IdLivroRecebido,IdVendedor,IdComprador,Estado,Timestamp")] Trocas trocas)
        {
            if (id != trocas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trocas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrocasExists(trocas.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdComprador"] = new SelectList(_context.Users, "Id", "Id", trocas.IdComprador);
            ViewData["IdLivroDado"] = new SelectList(_context.Livros, "Id", "Autor", trocas.IdLivroDado);
            ViewData["IdLivroRecebido"] = new SelectList(_context.Livros, "Id", "Autor", trocas.IdLivroRecebido);
            ViewData["IdVendedor"] = new SelectList(_context.Users, "Id", "Id", trocas.IdVendedor);
            return View(trocas);
        }

        // GET: Trocas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trocas = await _context.Trocas
                .Include(t => t.Comprador)
                .Include(t => t.LivroDado)
                .Include(t => t.LivroRecebido)
                .Include(t => t.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trocas == null)
            {
                return NotFound();
            }

            return View(trocas);
        }

        // POST: Trocas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trocas = await _context.Trocas.FindAsync(id);
            if (trocas != null)
            {
                _context.Trocas.Remove(trocas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrocasExists(int id)
        {
            return _context.Trocas.Any(e => e.Id == id);
        }
    }
}
