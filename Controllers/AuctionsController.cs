using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Auktioner.Data;
using MVC_Auktioner.Models;

namespace MVC_Auktioner.Controllers;

public class AuctionsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuctionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var auctions = await _db.Auctions.ToListAsync();
        return View(auctions);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuctionModel auction)
    {
        auction.InventoryId = auction.InventoryId.ToUpperInvariant();

        if (auction.StartingPrice < auction.PurchaseCost)
        {
            ModelState.AddModelError(nameof(auction.StartingPrice), "Starting price cannot be below purchase cost.");
        }

        if (!ModelState.IsValid)
        {
            return View(auction);
        }

        var exists = await _db.Auctions.AnyAsync(a => a.InventoryId == auction.InventoryId);
        if (exists)
        {
            ModelState.AddModelError(nameof(auction.InventoryId), "This InventoryId already exists.");
            return View(auction);
        }

        auction.CreationDate = DateTime.UtcNow;
        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
