using Microsoft.AspNetCore.Mvc;
using PeluShop.Services;

namespace Projet_API.Controllers;

public class AdminController : Controller
{
    private readonly IPelucheService _pelucheService;
    private readonly IAuthService _authService;

    public AdminController(IPelucheService pelucheService, IAuthService authService)
    {
        _pelucheService = pelucheService;
        _authService = authService;
    }

    public IActionResult Dashboard()
    {
        if (!User.Identity.IsAuthenticated || User.IsInRole("User"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        return View();
    }
    public IActionResult Peluches()
    {
        if (!User.Identity.IsAuthenticated || User.IsInRole("User"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        var peluches = _pelucheService.GetAll();
        return View(peluches);
    }

    public IActionResult Utilisateurs()
    {
        if (!User.Identity.IsAuthenticated || User.IsInRole("User"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        var utilisateurs = _authService.GetAllUsers();
        return View(utilisateurs);
    }

    [HttpGet]
    public IActionResult AjouterPeluche()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AjouterPeluche(Peluche peluche)
    {
        if (ModelState.IsValid)
        {
            _pelucheService.Add(peluche);
            return RedirectToAction("Peluches");
        }

        return View(peluche);
    }

    [HttpGet]
    public IActionResult ModifierPeluche(int id)
    {
        var peluche = _pelucheService.GetById(id);
        return View(peluche);
    }

    [HttpPost]
    public IActionResult ModifierPeluche(Peluche peluche)
    {
        if (ModelState.IsValid)
        {
            _pelucheService.Update(peluche);
            return RedirectToAction("Peluches");
        }
        return View(peluche);
    }

    [HttpGet]
    public IActionResult SupprimerPeluche(int id)
    {
        if (!_pelucheService.Delete(id))
        {
            TempData["Erreur"] = "Cette peluche est utilisée dans une commande et ne peut pas être supprimée.";
        }
        return RedirectToAction("Peluches");
    }

    [HttpGet]
    public IActionResult SupprimerUtilisateur(int id)
    {
        if (!_authService.DeleteUser(id))
        {
            TempData["Erreur"] = "Impossible de supprimer l'utilisateur Admin";
        }
        return RedirectToAction("Utilisateurs");
    }
}
