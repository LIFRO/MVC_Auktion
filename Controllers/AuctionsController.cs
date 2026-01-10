using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Auktioner.Data;
using MVC_Auktioner.Models;
using System.Security.Claims;

namespace MVC_Auktioner.Controllers;

[Authorize]
public class AuctionsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuctionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var auctions = await _db.Auctions
            .Include(a => a.Category)
            .AsNoTracking()
            .ToListAsync();
        return View(auctions);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuctionModel auction)
    {
        auction.InventoryId = auction.InventoryId.ToUpperInvariant();
        ModelState.Remove(nameof(auction.Seller));

        var sellerName = User?.Identity?.Name ?? User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        auction.Seller = sellerName;

        if (auction.StartingPrice < auction.PurchaseCost)
        {
            ModelState.AddModelError(nameof(auction.StartingPrice), "Starting price cannot be below purchase cost.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(auction.CategoryId);
            return View(auction);
        }

        var exists = await _db.Auctions.AnyAsync(a => a.InventoryId == auction.InventoryId);
        if (exists)
        {
            ModelState.AddModelError(nameof(auction.InventoryId), "This InventoryId already exists.");
            await PopulateCategoriesAsync(auction.CategoryId);
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

    [HttpPost, Authorize(Roles = "IsAResponsible")]
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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);
        if (auction == null)
        {
            return NotFound();
        }

        if (auction.Sold)
        {
            return RedirectToAction(nameof(SoldEdit), new {id});
        }

        var model = new EditAuctionModel
        {
            Id = auction.Id,
            InventoryId = auction.InventoryId,
            Decade = auction.Decade,
            CategoryId = auction.CategoryId,
            Title = auction.Title,
            Description = auction.Description,
            Location = auction.Location,
            StartingPrice = auction.StartingPrice,
            PurchaseCost = auction.PurchaseCost
        };
        await PopulateCategoriesAsync(model.CategoryId);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditAuctionModel model)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == model.Id);
        if (auction == null)
        {
            return NotFound();
        }
        if (auction.Sold)
        {
            return RedirectToAction(nameof(SoldEdit), new {id = model.Id});
        }

        model.Seller = auction.Seller;
        ModelState.Remove(nameof(model.Seller));
        model.InventoryId = model.InventoryId.ToUpperInvariant();

        if (model.StartingPrice < model.PurchaseCost)
        {
            ModelState.AddModelError(nameof(model.StartingPrice), "Starting price cannot be below purchase cost.");
        }

        var inventoryExists = await _db.Auctions
            .AnyAsync(a => a.InventoryId == model.InventoryId && a.Id != model.Id);
        if (inventoryExists)
        {
            ModelState.AddModelError(nameof(model.InventoryId), "This InventoryId already exists.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        auction.InventoryId = model.InventoryId;
        auction.Decade = model.Decade;
        auction.CategoryId = model.CategoryId;
        auction.Title = model.Title;
        auction.Description = model.Description;
        auction.Location = model.Location;
        auction.Seller = auction.Seller;
        auction.StartingPrice = model.StartingPrice;
        auction.PurchaseCost = model.PurchaseCost;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SoldEdit(int id)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        if (!auction.Sold)
        {
            return RedirectToAction(nameof(Edit), new {id});
        }

        var model = new SoldEditAuctionModel
        {
            Id = auction.Id,
            Decade = auction.Decade,
            CategoryId = auction.CategoryId,
            Title = auction.Title,
            Description = auction.Description,
            Location = auction.Location,
            FinalPrice = auction.FinalPrice ?? auction.StartingPrice,
            Sold = auction.Sold
        };

        await PopulateCategoriesAsync(model.CategoryId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoldEdit(SoldEditAuctionModel model)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == model.Id);
        if (auction == null)
        {
            return NotFound();
        }

        if (!auction.Sold)
        {
            return RedirectToAction(nameof(Edit), new {id = model.Id});
        }

        if (model.Sold && model.FinalPrice < auction.StartingPrice)
        {
            ModelState.AddModelError(nameof(model.FinalPrice),
                "Final price cannot be lower than starting price");
        }

        ModelState.Remove(nameof(model.Seller));
        model.Seller = auction.Seller;

        if (!ModelState.IsValid)
        {
            model.Decade = auction.Decade;
            model.CategoryId = auction.CategoryId;
            model.Title = auction.Title;
            model.Description = auction.Description;
            model.Location = auction.Location;
            model.FinalPrice = auction.FinalPrice ?? auction.StartingPrice;
            model.Sold = auction.Sold;
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        auction.Decade = model.Decade;
        auction.CategoryId = model.CategoryId;
        auction.Title = model.Title;
        auction.Description = model.Description;
        auction.Location = model.Location;
        auction.Seller = auction.Seller;
        if (model.Sold)
        {
            auction.FinalPrice = model.FinalPrice;
            auction.Sold = true;
        }
        else
        {
            auction.FinalPrice = null;
            auction.Sold = false;
        }

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    private async Task PopulateCategoriesAsync(int? selectedCategoryId = null)
    {
        var categories = await _db.Category
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        ViewBag.CategoryList = new SelectList(categories, nameof(CategoryModel.Id),
            nameof(CategoryModel.CategoryName), selectedCategoryId);
    }
}
