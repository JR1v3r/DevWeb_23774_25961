using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Controllers
{
    public class TrocasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : Controller
    {
        // GET: Trocas
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = context.Trocas.Include(t => t.Comprador).Include(t => t.LivroDado).Include(t => t.LivroRecebido).Include(t => t.Vendedor);
            return View(await applicationDbContext.ToListAsync());
        }
        
        
        // GET: Livros/MyTrades
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> MyTrades()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var minhasTrocas = await context.Trocas
                .Include(t => t.LivroDado)
                .Include(t => t.Vendedor)
                .Include(t => t.Comprador)
                .Include(t => t.LivroRecebido)
                .Where(t => t.IdVendedor == user.Id || t.IdComprador == user.Id)
                .ToListAsync();

            return View(minhasTrocas);
        }
        
        
        // GET: Trocas/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            var trocas = await context.Trocas
                .Include(t => t.Comprador)
                .Include(t => t.LivroDado)
                .Include(t => t.LivroRecebido)
                .Include(t => t.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (trocas == null)
            {
                return NotFound();
            }
            
            if (trocas.IdVendedor != user.Id 
                || trocas.IdComprador == user.Id)
            {
                return Forbid();
            }

            return View(trocas);
        }

        // GET: Trocas/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            ViewData["IdComprador"] = new SelectList(context.Users, "Id", "Id");
            ViewData["IdLivroDado"] = new SelectList(context.Livros, "Id", "Autor");
            ViewData["IdLivroRecebido"] = new SelectList(context.Livros, "Id", "Autor");
            ViewData["IdVendedor"] = new SelectList(context.Users, "Id", "Id");
            return View();
        }

        // POST: Trocas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLivroDado")] Trocas trocas)
        {
            var user = await userManager.GetUserAsync(User);
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
                context.Add(trocas);
                await context.SaveChangesAsync();
                return RedirectToAction("MyBooks", "Livros");
            }

            // Just in case validation fails... load ViewData again~
            ViewData["IdLivroDado"] = new SelectList(context.Livros, "Id", "Titulo", trocas.IdLivroDado);
            return RedirectToAction("MyBooks", "Livros");
        }


        // GET: Trocas/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trocas = await context.Trocas.FindAsync(id);
            if (trocas == null)
            {
                return NotFound();
            }
            ViewData["IdComprador"] = new SelectList(context.Users, "Id", "Id", trocas.IdComprador);
            ViewData["IdLivroDado"] = new SelectList(context.Livros, "Id", "Autor", trocas.IdLivroDado);
            ViewData["IdLivroRecebido"] = new SelectList(context.Livros, "Id", "Autor", trocas.IdLivroRecebido);
            ViewData["IdVendedor"] = new SelectList(context.Users, "Id", "Id", trocas.IdVendedor);
            return View(trocas);
        }

        // POST: Trocas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,User")]
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
                    context.Update(trocas);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrocasExists(trocas.Id))
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(MyTrades));
            }
            ViewData["IdComprador"] = new SelectList(context.Users, "Id", "Id", trocas.IdComprador);
            ViewData["IdLivroDado"] = new SelectList(context.Livros, "Id", "Autor", trocas.IdLivroDado);
            ViewData["IdLivroRecebido"] = new SelectList(context.Livros, "Id", "Autor", trocas.IdLivroRecebido);
            ViewData["IdVendedor"] = new SelectList(context.Users, "Id", "Id", trocas.IdVendedor);
            return View(trocas);
        }

        // GET: Trocas/TradeProposal/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> TradeProposal(int id)
        {
            var troca = await context.Trocas
                .Include(t => t.LivroDado)
                .Include(t => t.Vendedor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (troca == null)
                return NotFound();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // Filter user's books not in pending trade
            var livrosEmTroca = await context.Trocas
                .Where(t => t.IdComprador == user.Id || t.IdVendedor == user.Id)
                .Where(t => t.Estado == Trocas.EstadoTroca.Pendente)
                .Select(t => t.IdLivroDado)
                .ToListAsync();

            var meusLivrosDisponiveis = await context.Livros
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
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TradeProposal(int id, [Bind("IdLivroRecebido")] Trocas updatedTrade)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var troca = await context.Trocas.FindAsync(id);
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
                context.Update(troca);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(MyTrades));
            }

            // If there's a validation issue, re-render the view
            var livrosEmTroca = await context.Trocas
                .Where(t => t.IdComprador == user.Id || t.IdVendedor == user.Id)
                .Where(t => t.Estado == Trocas.EstadoTroca.Pendente)
                .Select(t => t.IdLivroDado)
                .ToListAsync();

            var meusLivrosDisponiveis = await context.Livros
                .Where(l => l.UserId == user.Id && !livrosEmTroca.Contains(l.Id))
                .ToListAsync();

            ViewBag.LivrosDisponiveis = new SelectList(meusLivrosDisponiveis, "Id", "Titulo", updatedTrade.IdLivroRecebido);
            return View(troca);
        }

        
        // GET: Trocas/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            
            var trocas = await context.Trocas
                .Include(t => t.Comprador)
                .Include(t => t.LivroDado)
                .Include(t => t.LivroRecebido)
                .Include(t => t.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (trocas == null)
            {
                return NotFound();
            }

            if (trocas.IdVendedor != user.Id){
                return Forbid();
            }
            
            return View(trocas);
        }

        // POST: Trocas/Delete/5
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var troca = await context.Trocas.FindAsync(id);
            if (troca == null)
                return NotFound();

            bool isVendedor = troca.IdVendedor == user.Id;
            bool isComprador = troca.IdComprador == user.Id;

            // 🍌 If you're the one who created the trade, you can delete it completely
            if (isVendedor)
            {
                context.Trocas.Remove(troca);
            }
            // 🍡 If you're the one who proposed a trade, nullify your proposal and revert status
            else if (isComprador)
            {
                troca.IdLivroRecebido = null;
                troca.IdComprador = null;
                troca.Estado = Trocas.EstadoTroca.Criada;

                context.Trocas.Update(troca);
            }
            else
            {
                return Forbid(); // you can't touch trades you're not part of >:c
            }

            await context.SaveChangesAsync();
            return RedirectToAction("MyBooks", "Livros");
        }
        
        
        // POST: Trocas/Aceitar/5
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aceitar(int id)
        {
            var troca = await context.Trocas
                .Include(t => t.LivroDado)
                .Include(t => t.LivroRecebido)
                .FirstOrDefaultAsync(t => t.Id == id);

            var user = await userManager.GetUserAsync(User);
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

                context.Update(livroDado);
                context.Update(livroRecebido);
                context.Update(troca);

                await context.SaveChangesAsync();
            }

            return RedirectToAction("MyTrades");
        }

        
        // POST: Trocas/Recusar/5
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recusar(int id)
        {
            var troca = await context.Trocas.FindAsync(id);
            var user = await userManager.GetUserAsync(User);

            if (troca == null || troca.IdVendedor != user.Id)
                return Forbid();

            if (troca.Estado != Trocas.EstadoTroca.Pendente)
                return BadRequest();

            troca.Estado = Trocas.EstadoTroca.Recusada;

            context.Update(troca);
            await context.SaveChangesAsync();

            return RedirectToAction("MyTrades");
        }
        
        
        private bool TrocasExists(int id)
        {
            return context.Trocas.Any(e => e.Id == id);
        }
    }
}
