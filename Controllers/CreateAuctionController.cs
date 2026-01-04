using Microsoft.AspNetCore.Mvc;

namespace MVC_Auktioner.Controllers;

public class CreateAuctionController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
