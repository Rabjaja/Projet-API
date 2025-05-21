using Microsoft.AspNetCore.Mvc;
using PeluShop.Services;
using System.Security.Claims;

public class ShopController : Controller
{
    private readonly IPelucheService _pelucheService;
    private readonly IPanierService _panierService;
    private readonly ICommandeService _commandeService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShopController(IPelucheService pelucheService, IPanierService panierService, ICommandeService commandeService, IHttpContextAccessor httpContextAccessor)
    {
        _pelucheService = pelucheService;
        _panierService = panierService;
        _commandeService = commandeService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }
        var peluches = _pelucheService.GetAll();

        var panier = _panierService.GetPanier();
        ViewBag.Panier = panier;

        return View(peluches);
    }

    public IActionResult Customize(int id)
    {
        if (User.Identity?.IsAuthenticated == false)
            return RedirectToAction("Register", "Account");
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }
        var peluche = _pelucheService.GetById(id);
        if (peluche == null)
            return NotFound();

        var vm = new PersonnalisationViewModel
        {
            PelucheId = peluche.Id,
            NomBrode = "",
            Couleur = "",
            TypeCouture = ""
        };

        ViewData["NomPeluche"] = peluche.Nom;

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Customize(PersonnalisationViewModel model)
    {
        var peluche = _pelucheService.GetById(model.PelucheId);

        if (!ModelState.IsValid)
        {
            ViewData["NomPeluche"] = peluche?.Nom;
            return View(model);
        }

        var ligne = new LignePanier
        {
            PelucheId = peluche.Id,
            Nom = peluche.Nom,
            Prix = peluche.Prix,
            NomBrode = model.NomBrode,
            Couleur = model.Couleur,
            TypeCouture = model.TypeCouture
        };

        _panierService.AjouterAuPanier(ligne);

        TempData["Message"] = $"« {peluche.Nom} » ajoutée au panier !";

        return RedirectToAction("Index");
    }

    public IActionResult Details(int id)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }
        var peluche = _pelucheService.GetById(id);
        if (peluche == null)
        {
            return NotFound();
        }

        return View(peluche);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PasserCommande()
    {
        var panier = _panierService.GetPanier();

        if (!panier.Any()) return RedirectToAction("Index");

        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);
        _commandeService.PasserCommande(userId, panier);

        _panierService.ViderPanier();

        TempData["Message"] = "Commande enregistrée avec succès !";
        return RedirectToAction("Index");
    }
    public IActionResult MesCommandes()
    {
        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        var commandes = _commandeService.GetCommandesByUser(userId);

        var peluchesDict = new Dictionary<int, Peluche>();

        foreach (var commande in commandes)
        {
            foreach (var ligne in commande.Lignes)
            {
                if (!peluchesDict.ContainsKey(ligne.PelucheId))
                {
                    var peluche = _pelucheService.GetById(ligne.PelucheId);
                    if (peluche != null)
                    {
                        peluchesDict.Add(peluche.Id, peluche);
                    }
                }
            }
        }

        ViewData["Peluches"] = peluchesDict;

        return View(commandes);
    }
}
