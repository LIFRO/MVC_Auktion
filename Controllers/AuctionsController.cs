using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
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

    [HttpGet]
    public async Task<IActionResult> Sell(int id)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);
        if ( auction is null)
        {
            return NotFound();
        }

        if (auction.Sold)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new SellAuctionModel
        {
            Id = auction.Id,
            InventoryId = auction.InventoryId,
            StartingPrice = auction.StartingPrice,
            FinalPrice = auction.FinalPrice ?? auction.StartingPrice
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Sell(SellAuctionModel model)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync( a => a.Id  == model.Id);
        if (auction == null)
        {
            return NotFound();
        }
        
        if (model.FinalPrice < auction.StartingPrice)
        {
            ModelState.AddModelError(nameof(model.FinalPrice),
                "Final price can not be lower than starting price");
        }

        if (!ModelState.IsValid)
        {
            model.InventoryId = auction.InventoryId;
            model.StartingPrice = auction.StartingPrice;
            return View(model);
        }

        auction.FinalPrice = model.FinalPrice;
        auction.Sold = true;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
