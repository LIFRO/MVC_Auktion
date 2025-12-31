using Microsoft.AspNetCore.Mvc;

namespace MVC_Auktioner.Controllers;

public class RegistrationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
