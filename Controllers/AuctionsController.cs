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

    public async Task<IActionResult> Create(AuctionModel auction)
    {
        if (!ModelState.IsValid)
        {
            return View(auction);
        }

        auction.CreatedAt = DateTime.UtcNow;
        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
