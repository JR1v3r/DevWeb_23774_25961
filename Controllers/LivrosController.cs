using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Controllers
{
    public class LivrosController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        // GET: Livros
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string titulo, string autor, string isbn, string sortOrder)
        {
            var query = context.Livros.Include(l => l.User).AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(titulo))
                query = query.Where(l => l.Titulo.Contains(titulo));
            if (!string.IsNullOrEmpty(autor))
                query = query.Where(l => l.Autor.Contains(autor));
            if (!string.IsNullOrEmpty(isbn))
                query = query.Where(l => l.ISBN.Contains(isbn));

            // Sorting
            query = sortOrder switch
            {
                "titulo_asc" => query.OrderBy(l => l.Titulo),
                "titulo_desc" => query.OrderByDescending(l => l.Titulo),
                "autor_asc" => query.OrderBy(l => l.Autor),
                "autor_desc" => query.OrderByDescending(l => l.Autor),
                "isbn_asc" => query.OrderBy(l => l.ISBN),
                "isbn_desc" => query.OrderByDescending(l => l.ISBN),
                _ => query.OrderBy(l => l.Id)
            };

            var livros = await query.ToListAsync();
            return View(livros);
        }
        
        // GET: MyBooks
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> MyBooks()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var meusLivros = await context.Livros
                .Where(l => l.UserId == user.Id)
                .ToListAsync();

            var trocasAtivas = await context.Trocas
                .Where(t => t.Estado == Trocas.EstadoTroca.Criada || t.Estado == Trocas.EstadoTroca.Pendente)
                .ToListAsync();

            var model = (
                Livros: meusLivros.AsEnumerable(),
                Trocas: trocasAtivas.AsEnumerable()
            );
            return View(model);

        }
        
        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livros = await context.Livros
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livros == null)
            {
                return NotFound();
            }

            return View(livros);
        }

        // GET: Livros/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(context.Users, "Id", "Id");
            return View();
        }

        // POST: Livros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Livros livros, IFormFile capaFile)
        {
            if (capaFile.Length == 0)
            {
                ModelState.AddModelError("CapaFile", "Por favor, selecione uma imagem para a capa.");
                return View(livros);
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

                var ext = Path.GetExtension(capaFile.FileName).ToLowerInvariant();
                string[] permittedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

                if (!permittedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("CapaFile", "Apenas imagens (.jpg, .jpeg, .png, .webp) são permitidas.");
                    return View(livros);
                }

                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadsPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await capaFile.CopyToAsync(stream);
                }

                livros.Capa = "/uploads/" + fileName;
                livros.UserId = user.Id;
                livros.IsActive = true;

                context.Add(livros);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(MyBooks));
            }

            return View(livros);
        }


        // GET: Livros/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var livros = await context.Livros.FindAsync(id);
            
            if (livros == null)
            {
                return NotFound();
            }
            
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            if (livros.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            ViewData["UserId"] = new SelectList(context.Users, "Id", "Id", livros.UserId);
            return View(livros);
        }

        // POST: Livros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Livros livros, IFormFile capaFile)
        {
            if (id != livros.Id)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (livros.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Find the existing book in DB
                    var existingLivro = await context.Livros.FindAsync(id);
                    if (existingLivro == null) return NotFound();

                    existingLivro.Titulo = livros.Titulo;
                    existingLivro.Autor = livros.Autor;
                    existingLivro.ISBN = livros.ISBN;
                    existingLivro.Sinopse = livros.Sinopse;

                    if (capaFile != null && capaFile.Length > 0)
                    {
                        var ext = Path.GetExtension(capaFile.FileName).ToLowerInvariant();
                        string[] permittedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

                        if (!permittedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("CapaFile", "Apenas imagens (.jpg, .jpeg, .png, .webp) são permitidas.");
                            return View(livros);
                        }

                        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        Directory.CreateDirectory(uploadsPath);

                        var fileName = Guid.NewGuid() + ext;
                        var filePath = Path.Combine(uploadsPath, fileName);

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await capaFile.CopyToAsync(stream);
                        }

                        existingLivro.Capa = "/uploads/" + fileName;
                    }

                    context.Update(existingLivro);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyBooks));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivrosExists(livros.Id)) return NotFound();
                    else throw;
                }
            }

            return View(livros);
        }



        // GET: Livros/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
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
            
            var livros = await context.Livros
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (livros == null)
            {
                return NotFound();
            }
            
            if (livros.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(livros);
        }

        // POST: Livros/Delete/5
        [Authorize(Roles = "Admin,User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var livro = await context.Livros.FindAsync(id);

            if (livro == null)
            {
                return NotFound();
            }

            if (livro.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            context.Livros.Remove(livro);
            await context.SaveChangesAsync();
            return RedirectToAction("MyBooks");
        }

        private bool LivrosExists(int id)
        {
            return context.Livros.Any(e => e.Id == id);
        }
    }
}
