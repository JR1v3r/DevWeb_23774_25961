using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using DevWeb_23774_25961.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;


namespace DevWeb_23774_25961.Controllers
{
    public class TrocasController(ApplicationDbContext context, UserManager<IdentityUser> userManager, EmailSender emailSender, IHubContext<TradeHub> hubContext)
        : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder)
        {
            var query = context.Trocas
                .Include(t => t.Vendedor).AsQueryable()
                .Include(t => t.Comprador).AsQueryable()
                .Include(t => t.LivroDado).AsQueryable()
                .Include(t => t.LivroRecebido).AsQueryable();
            
            // Sorting
            query = sortOrder switch
            {
                "LivroDado_asc" => query.OrderBy(t => t.LivroDado.Titulo),
                "LivroDado_desc" => query.OrderByDescending(t => t.LivroDado.Titulo),
                "LivroRecebido_asc" => query.OrderBy(t => t.LivroRecebido.Titulo),
                "LivroRecebido_desc" => query.OrderByDescending(t => t.LivroRecebido.Titulo),
                "Vendedor_asc" => query.OrderBy(t => t.Vendedor.UserName),
                "Vendedor_desc" => query.OrderByDescending(t => t.Vendedor.UserName),
                "Comprador_asc" => query.OrderBy(t => t.Comprador.UserName),
                "Comprador_desc" => query.OrderByDescending(t => t.Comprador.UserName),
                "Data_asc" => query.OrderBy(t => t.Timestamp),
                "Data_desc" => query.OrderByDescending(t => t.Timestamp),
                _ => query.OrderBy(t => t.Id)
            };

            var livros = await query.ToListAsync();
            return View(livros);
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

            // Get book details for the email
            var livro = await context.Livros.FindAsync(trocas.IdLivroDado);
            if (livro == null)
            {
                ModelState.AddModelError("", "O livro selecionado não existe.");
                ViewData["IdLivroDado"] = new SelectList(context.Livros, "Id", "Titulo", trocas.IdLivroDado);
                return View(trocas);
            }

            // Set remaining properties
            trocas.IdVendedor = user.Id;
            trocas.Estado = Trocas.EstadoTroca.Criada;
            trocas.Timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                context.Add(trocas);
                await context.SaveChangesAsync();

                // Prepare and send email
                var subject = $@"Confirmação de Troca #{trocas.Id} - Book'n'Swap";
                var body = $@"
                    Olá {user.UserName}!

                    A tua troca foi criada com sucesso. Aqui estão os detalhes:

                    ID da troca: #{trocas.Id}
                    Livro: {livro.Titulo}
                    Autor: {livro.Autor}
                    ISBN: {livro.ISBN}
                    Sinopse: {livro.Sinopse}
                    Data da Troca: {trocas.Timestamp.ToString()}

                    Iremos notificá-lo quando outro utilizador estiver interessado.

                    Obrigado por usar a Book'n'Swap! ✨
                ";

                await emailSender.SendEmailAsync(user.Email, subject, body);

                return RedirectToAction("MyBooks", "Livros");
            }

            // Validation failed
            ViewData["IdLivroDado"] = new SelectList(context.Livros, "Id", "Titulo", trocas.IdLivroDado);
            return View(trocas);
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

            var livroProposto = await context.Livros.FindAsync(updatedTrade.IdLivroRecebido);
            var livroOriginal = await context.Livros.FindAsync(troca.IdLivroDado);
            var donoTroca = await userManager.FindByIdAsync(troca.IdVendedor);

            if (livroProposto == null || livroOriginal == null || donoTroca == null)
            {
                return NotFound("Dados inválidos para a troca.");
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

                // ✨ Email to Trade Owner ✨
                var subjectDono = "Nova Proposta de Troca no Book'n'Swap!";
                var bodyDono = $@"
                    Olá {donoTroca.UserName},

                    Alguém fez uma proposta para a tua troca #{troca.Id}!

                    📖 Livro que ofereceste: {livroOriginal.Titulo}
                    📚 Livro proposto: {livroProposto.Titulo}
                    ✍️ Proponente: {user.UserName}

                    Podes ver os detalhes no site.

                    Obrigado por usar o Book'n'Swap!
                            ";

                await emailSender.SendEmailAsync(donoTroca.Email, subjectDono, bodyDono);

                // ✨ Email to Proposer ✨
                var subjectProposer = "Proposta de Troca Submetida!";
                var bodyProposer = $@"
                    Olá {user.UserName},

                    A tua proposta para a troca #{troca.Id} foi registada com sucesso.

                    📖 Livro original: {livroOriginal.Titulo}
                    📚 Livro que propuseste: {livroProposto.Titulo}

                    O dono da troca será notificado e, se aceitar, iremos informar-te!

                    Obrigado por usar o Book'n'Swap!
                            ";

                await emailSender.SendEmailAsync(user.Email, subjectProposer, bodyProposer);

                await hubContext.Clients.User(troca.IdVendedor).SendAsync(
                    "ReceiveNotification",
                    $"Alguém propôs um livro na sua troca #{troca.Id}!"
                );
                
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

            var livroDado = troca.LivroDado;
            var livroRecebido = troca.LivroRecebido;

            if (livroDado != null && livroRecebido != null)
            {
                var comprador = await userManager.FindByIdAsync(troca.IdComprador);
                var vendedor = user;

                // Swap ownership
                var tempOwner = livroDado.UserId;
                livroDado.UserId = comprador.Id;
                livroRecebido.UserId = vendedor.Id;

                troca.Estado = Trocas.EstadoTroca.Aceite;

                context.Update(livroDado);
                context.Update(livroRecebido);
                context.Update(troca);

                await context.SaveChangesAsync();

                // ✨ Email to Buyer ✨
                var subjectComprador = "A sua proposta foi aceite!";
                var bodyComprador = $@"
                    Olá {comprador.UserName},

                    Boas notícias! A sua proposta na troca #{troca.Id} foi aceite.

                    Livro que recebes: {livroDado.Titulo}
                    Livro que entrega: {livroRecebido.Titulo}

                    Já pode ver o seu novo livro na sua coleção.

                    Obrigado por usar o Book'n'Swap!
                            ";

                await emailSender.SendEmailAsync(comprador.Email, subjectComprador, bodyComprador);

                // ✨ Email to Seller ✨
                var subjectVendedor = "Troca concluida com sucesso!";
                var bodyVendedor = $@"
                    Olá {vendedor.UserName},

                    Confirmamos que a proposta à sua troca #{troca.Id} foi aceite.

                    Livro que entrega: {livroDado.Titulo}
                    Livro que recebe: {livroRecebido.Titulo}

                    Já pode ver o seu novo livro na sua coleção.

                    Obrigado por usar o Book'n'Swap!
                            ";

                await emailSender.SendEmailAsync(vendedor.Email, subjectVendedor, bodyVendedor);
                
                await hubContext.Clients.User(troca.IdComprador).SendAsync(
                    "ReceiveNotification",
                    $"A sua proposta na troca #{troca.Id} foi aceite!"
                );

            }

            return RedirectToAction("MyTrades");
        }


        
        // POST: Trocas/Recusar/5
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recusar(int id)
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

            troca.Estado = Trocas.EstadoTroca.Recusada;

            context.Update(troca);
            await context.SaveChangesAsync();

            // ✨ Email to Buyer (Proposer)
            if (troca.IdComprador != null)
            {
                var comprador = await userManager.FindByIdAsync(troca.IdComprador);

                var subjectComprador = "A sua proposta foi recusada";
                var bodyComprador = $@"
                    Olá {comprador.UserName},

                    Infelizmente, a sua proposta para a troca #{troca.Id} foi recusada.

                    Livro que propôs: {troca.LivroRecebido?.Titulo ?? "(livro desconhecido)"}
                    Livro do utilizador: {troca.LivroDado?.Titulo ?? "(livro desconhecido)"}

                    Continue a explorar novas oportunidades na Book'n'Swap ✨
                    Obrigado por usar o nosso serviço!
                            ";

                await emailSender.SendEmailAsync(comprador.Email, subjectComprador, bodyComprador);
            }

            // ✨ Email to Owner (Rejecting User)
            var subjectVendedor = "Recusou uma proposta de troca";
            var bodyVendedor = $@"
                Olá {user.UserName},

                Confirmamos que a proposta para a troca #{troca.Id} foi por si recusada.

                O seu livro: {troca.LivroDado?.Titulo ?? "(livro desconhecido)"}
                Livro proposto: {troca.LivroRecebido?.Titulo ?? "(livro desconhecido)"}

                Obrigado por continuar a usar o Book'n'Swap!
                    ";

            await emailSender.SendEmailAsync(user.Email, subjectVendedor, bodyVendedor);

            await hubContext.Clients.User(troca.IdComprador).SendAsync(
                "ReceiveNotification",
                $"A sua proposta na troca #{troca.Id} foi recusada."
            );

            
            return RedirectToAction("MyTrades");
        }


        
        
        private bool TrocasExists(int id)
        {
            return context.Trocas.Any(e => e.Id == id);
        }
    }
}
