public interface ICommandeService
{
    void PasserCommande(int userId, List<LignePanier> panier);
    List<Commande> GetCommandesByUser(int userId);

}
