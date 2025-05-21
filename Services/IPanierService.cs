using System.Collections.Generic;

public interface IPanierService
{
    List<LignePanier> GetPanier();
    void AjouterAuPanier(LignePanier ligne);
    void ViderPanier();
}
