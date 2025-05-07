using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;

namespace DevWeb_23774_25961.Controllers
{
    public class UsersBooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersBooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UsersBooks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UsersBooks.Include(u => u.Book).Include(u => u.Creator).Include(u => u.Updater);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UsersBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersBooks = await _context.UsersBooks
                .Include(u => u.Book)
                .Include(u => u.Creator)
                .Include(u => u.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersBooks == null)
            {
                return NotFound();
            }

            return View(usersBooks);
        }

        // GET: UsersBooks/Create
        public IActionResult Create()
        {
            ViewData["Books_Id"] = new SelectList(_context.Books, "Id", "Autor");
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UsersBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Books_Id,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,IsActive")] UsersBooks usersBooks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usersBooks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Books_Id"] = new SelectList(_context.Books, "Id", "Autor", usersBooks.Books_Id);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", usersBooks.CreatedBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", usersBooks.UpdatedBy);
            return View(usersBooks);
        }

        // GET: UsersBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersBooks = await _context.UsersBooks.FindAsync(id);
            if (usersBooks == null)
            {
                return NotFound();
            }
            ViewData["Books_Id"] = new SelectList(_context.Books, "Id", "Autor", usersBooks.Books_Id);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", usersBooks.CreatedBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", usersBooks.UpdatedBy);
            return View(usersBooks);
        }

        // POST: UsersBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Books_Id,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,IsActive")] UsersBooks usersBooks)
        {
            if (id != usersBooks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersBooks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersBooksExists(usersBooks.Id))
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
            ViewData["Books_Id"] = new SelectList(_context.Books, "Id", "Autor", usersBooks.Books_Id);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", usersBooks.CreatedBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", usersBooks.UpdatedBy);
            return View(usersBooks);
        }

        // GET: UsersBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersBooks = await _context.UsersBooks
                .Include(u => u.Book)
                .Include(u => u.Creator)
                .Include(u => u.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersBooks == null)
            {
                return NotFound();
            }

            return View(usersBooks);
        }

        // POST: UsersBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usersBooks = await _context.UsersBooks.FindAsync(id);
            if (usersBooks != null)
            {
                _context.UsersBooks.Remove(usersBooks);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersBooksExists(int id)
        {
            return _context.UsersBooks.Any(e => e.Id == id);
        }
    }
}
