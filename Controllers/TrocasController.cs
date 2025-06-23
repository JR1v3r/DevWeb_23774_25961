using System.Text.Json;
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
                .Include(t => t.Comprador)
                .Include(t => t.LivroRecebido)
                .Where(t => t.IdVendedor == user.Id || t.IdComprador == user.Id)
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
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Set remaining properties
            trocas.IdVendedor = user.Id;
            trocas.Estado = Trocas.EstadoTroca.Criada;
            trocas.Timestamp = DateTime.Now;

            // Optional fields will stay null: IdComprador & IdLivroRecebido

            if (ModelState.IsValid)
            {
                _context.Add(trocas);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyBooks", "Livros");
            }

            // Just in case validation fails... load ViewData again~
            ViewData["IdLivroDado"] = new SelectList(_context.Livros, "Id", "Titulo", trocas.IdLivroDado);
            return RedirectToAction("MyBooks", "Livros");
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

        // GET: Trocas/TradeProposal/5
        public async Task<IActionResult> TradeProposal(int id)
        {
            var troca = await _context.Trocas
                .Include(t => t.LivroDado)
                .Include(t => t.Vendedor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (troca == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // Filter user's books not in pending trade
            var livrosEmTroca = await _context.Trocas
                .Where(t => t.IdComprador == user.Id || t.IdVendedor == user.Id)
                .Where(t => t.Estado == Trocas.EstadoTroca.Pendente)
                .Select(t => t.IdLivroDado)
                .ToListAsync();

            var meusLivrosDisponiveis = await _context.Livros
                .Where(l => l.UserId == user.Id && !livrosEmTroca.Contains(l.Id))
                .Select(l => new
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Autor = l.Autor,
                    Capa = l.Capa
                })
                .ToListAsync();

            ViewBag.LivrosDisponiveisJson = JsonSerializer.Serialize(meusLivrosDisponiveis);

            return View(troca);
        }




        // POST: Trocas/TradeProposal/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TradeProposal(int id, [Bind("IdLivroRecebido")] Trocas updatedTrade)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var troca = await _context.Trocas.FindAsync(id);
            if (troca == null)
            {
                return NotFound();
            }

            // Update the existing trade with the proposal
            troca.IdLivroRecebido = updatedTrade.IdLivroRecebido;
            troca.IdComprador = user.Id;
            troca.Estado = Trocas.EstadoTroca.Pendente;
            troca.Timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Update(troca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If there's a validation issue, re-render the view
            var livrosEmTroca = await _context.Trocas
                .Where(t => t.IdComprador == user.Id || t.IdVendedor == user.Id)
                .Where(t => t.Estado == Trocas.EstadoTroca.Pendente)
                .Select(t => t.IdLivroDado)
                .ToListAsync();

            var meusLivrosDisponiveis = await _context.Livros
                .Where(l => l.UserId == user.Id && !livrosEmTroca.Contains(l.Id))
                .ToListAsync();

            ViewBag.LivrosDisponiveis = new SelectList(meusLivrosDisponiveis, "Id", "Titulo", updatedTrade.IdLivroRecebido);
            return View(troca);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var troca = await _context.Trocas.FindAsync(id);
            if (troca == null)
                return NotFound();

            bool isVendedor = troca.IdVendedor == user.Id;
            bool isComprador = troca.IdComprador == user.Id;

            // 🍌 If you're the one who created the trade, you can delete it completely
            if (isVendedor)
            {
                _context.Trocas.Remove(troca);
            }
            // 🍡 If you're the one who proposed a trade, nullify your proposal and revert status
            else if (isComprador)
            {
                troca.IdLivroRecebido = null;
                troca.IdComprador = null;
                troca.Estado = Trocas.EstadoTroca.Criada;

                _context.Trocas.Update(troca);
            }
            else
            {
                return Forbid(); // you can't touch trades you're not part of >:c
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyBooks", "Livros");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aceitar(int id)
        {
            var troca = await _context.Trocas
                .Include(t => t.LivroDado)
                .Include(t => t.LivroRecebido)
                .FirstOrDefaultAsync(t => t.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (troca == null || troca.IdVendedor != user.Id)
                return Forbid();

            if (troca.Estado != Trocas.EstadoTroca.Pendente)
                return BadRequest();

            // 👯 Swap ownerships!
            var livroDado = troca.LivroDado;
            var livroRecebido = troca.LivroRecebido;

            if (livroDado != null && livroRecebido != null)
            {
                var compradorId = troca.IdComprador;

                // swap ownership
                var tempOwner = livroDado.UserId;
                livroDado.UserId = compradorId;
                livroRecebido.UserId = user.Id;

                troca.Estado = Trocas.EstadoTroca.Aceite;

                _context.Update(livroDado);
                _context.Update(livroRecebido);
                _context.Update(troca);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyTrades");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recusar(int id)
        {
            var troca = await _context.Trocas.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (troca == null || troca.IdVendedor != user.Id)
                return Forbid();

            if (troca.Estado != Trocas.EstadoTroca.Pendente)
                return BadRequest();

            troca.Estado = Trocas.EstadoTroca.Recusada;

            _context.Update(troca);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyTrades");
        }

        
        private bool TrocasExists(int id)
        {
            return _context.Trocas.Any(e => e.Id == id);
        }
    }
}
