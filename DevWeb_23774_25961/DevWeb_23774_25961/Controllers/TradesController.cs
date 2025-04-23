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
    public class TradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trades
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Trades.Include(t => t.InventorySlot);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Trades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trades = await _context.Trades
                .Include(t => t.InventorySlot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trades == null)
            {
                return NotFound();
            }

            return View(trades);
        }

        // GET: Trades/Create
        public IActionResult Create()
        {
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "Users_Id");
            return View();
        }

        // POST: Trades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsersBooks_Id,PreviousOwner,CurrentOwner,Status")] Trades trades)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trades);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "Users_Id", trades.UsersBooks_Id);
            return View(trades);
        }

        // GET: Trades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trades = await _context.Trades.FindAsync(id);
            if (trades == null)
            {
                return NotFound();
            }
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "Users_Id", trades.UsersBooks_Id);
            return View(trades);
        }

        // POST: Trades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsersBooks_Id,PreviousOwner,CurrentOwner,Status")] Trades trades)
        {
            if (id != trades.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trades);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TradesExists(trades.Id))
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
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "Users_Id", trades.UsersBooks_Id);
            return View(trades);
        }

        // GET: Trades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trades = await _context.Trades
                .Include(t => t.InventorySlot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trades == null)
            {
                return NotFound();
            }

            return View(trades);
        }

        // POST: Trades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trades = await _context.Trades.FindAsync(id);
            if (trades != null)
            {
                _context.Trades.Remove(trades);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TradesExists(int id)
        {
            return _context.Trades.Any(e => e.Id == id);
        }
    }
}
