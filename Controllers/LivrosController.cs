using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Controllers
{
    public class LivrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        
        public LivrosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Livros
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Livros.Include(l => l.User);
            return View(await applicationDbContext.ToListAsync());
        }
        
        // GET: MyBooks
        public async Task<IActionResult> MyBooks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var meusLivros = await _context.Livros
                .Where(l => l.UserId == user.Id)
                .ToListAsync();

            var trocasAtivas = await _context.Trocas
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

            var livros = await _context.Livros
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livros == null)
            {
                return NotFound();
            }

            return View(livros);
        }

        // GET: Livros/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Livros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Livros livros, IFormFile CapaFile)
        {
            if (CapaFile == null || CapaFile.Length == 0)
            {
                ModelState.AddModelError("CapaFile", "Por favor, selecione uma imagem para a capa.");
                return View(livros);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

                string ext = Path.GetExtension(CapaFile.FileName).ToLowerInvariant();
                string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

                if (!permittedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("CapaFile", "Apenas imagens (.jpg, .jpeg, .png, .webp, .gif) são permitidas.");
                    return View(livros);
                }

                string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                string fileName = Guid.NewGuid() + ext;
                string filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CapaFile.CopyToAsync(stream);
                }

                livros.Capa = "/uploads/" + fileName;
                livros.UserId = user.Id;
                livros.IsActive = true;

                _context.Add(livros);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyBooks));
            }

            return View(livros);
        }


        // GET: Livros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livros = await _context.Livros.FindAsync(id);
            if (livros == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", livros.UserId);
            return View(livros);
        }

        // POST: Livros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Autor,ISBN,Sinopse,Capa,UserId,IsActive")] Livros livros)
        {
            if (id != livros.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livros);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivrosExists(livros.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MyBooks");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", livros.UserId);
            return View(livros);
        }

        // GET: Livros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livros = await _context.Livros
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livros == null)
            {
                return NotFound();
            }

            return View(livros);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livros = await _context.Livros.FindAsync(id);
            if (livros != null)
            {
                _context.Livros.Remove(livros);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyBooks");
        }

        private bool LivrosExists(int id)
        {
            return _context.Livros.Any(e => e.Id == id);
        }
    }
}
