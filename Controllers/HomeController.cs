using Microsoft.AspNetCore.Mvc;

namespace Projet_API.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Shop");

        return View();
    }
}

