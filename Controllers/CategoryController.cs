using Microsoft.AspNetCore.Mvc;
using MVC_Auktioner.Data;
using MVC_Auktioner.Models;

namespace MVC_Auktioner.Controllers;

public class CategoryController : Controller
{

    private readonly ApplicationDbContext _db;

    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CategoryModel category)
    {
        if (!ModelState.IsValid) return View(category);

        _db.Category.Add(category);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index", "auctions");
    }
}
