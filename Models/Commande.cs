public class Commande
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime DateCommande { get; set; } = DateTime.Now;
    public List<LigneCommande> Lignes { get; set; } = new();
}