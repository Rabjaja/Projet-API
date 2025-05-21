using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace PeluShop.Services;

public class PanierServiceSession : IPanierService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PanierServiceSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    //Par IA
    private string GetSessionKey()
    {
        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        return $"Panier_{username}";
    }

    public List<LignePanier> GetPanier()
    {
        var session = _httpContextAccessor.HttpContext.Session;
        var panierJson = session.GetString(GetSessionKey());

        if (string.IsNullOrEmpty(panierJson))
        {
            return new List<LignePanier>();
        }

        return JsonSerializer.Deserialize<List<LignePanier>>(panierJson);
    }

    public void AjouterAuPanier(LignePanier ligne)
    {
        var panier = GetPanier();

        var exist = panier.FirstOrDefault(l =>
            l.PelucheId == ligne.PelucheId &&
            l.NomBrode == ligne.NomBrode &&
            l.Couleur == ligne.Couleur &&
            l.TypeCouture == ligne.TypeCouture);

        panier.Add(ligne);

        var panierJson = JsonSerializer.Serialize(panier);
        _httpContextAccessor.HttpContext.Session.SetString(GetSessionKey(), panierJson);
    }
    public void ViderPanier()
    {
        _httpContextAccessor.HttpContext.Session.Remove(GetSessionKey());
    }
}
