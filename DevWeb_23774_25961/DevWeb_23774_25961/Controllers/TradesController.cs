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
            var applicationDbContext = _context.Trades.Include(t => t.Creator).Include(t => t.InventorySlot).Include(t => t.Updater);
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
                .Include(t => t.Creator)
                .Include(t => t.InventorySlot)
                .Include(t => t.Updater)
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "CreatedBy");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Trades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsersBooks_Id,Status,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,IsActive")] Trades trades)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trades);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", trades.CreatedBy);
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "CreatedBy", trades.UsersBooks_Id);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", trades.UpdatedBy);
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", trades.CreatedBy);
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "CreatedBy", trades.UsersBooks_Id);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", trades.UpdatedBy);
            return View(trades);
        }

        // POST: Trades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsersBooks_Id,Status,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,IsActive")] Trades trades)
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", trades.CreatedBy);
            ViewData["UsersBooks_Id"] = new SelectList(_context.UsersBooks, "Id", "CreatedBy", trades.UsersBooks_Id);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", trades.UpdatedBy);
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
                .Include(t => t.Creator)
                .Include(t => t.InventorySlot)
                .Include(t => t.Updater)
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
