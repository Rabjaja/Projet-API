using Npgsql;
using System.Data;
using Dapper;

public class CommandeService : ICommandeService
{
    private readonly IDbConnection _db;

    public CommandeService(IConfiguration config)
    {
        _db = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public void PasserCommande(int userId, List<LignePanier> panier)
    {
        var commandeId = _db.ExecuteScalar<int>(
            "INSERT INTO Commandes (UserId) VALUES (@UserId) RETURNING Id",
            new { UserId = userId });

        foreach (var ligne in panier)
        {
            _db.Execute(
                @"INSERT INTO LigneCommande (CommandeId, PelucheId, NomBrode, Couleur, TypeCouture) 
                  VALUES (@CommandeId, @PelucheId, @NomBrode, @Couleur, @TypeCouture)",
                new
                {
                    commandeId,
                    ligne.PelucheId,
                    ligne.NomBrode,
                    ligne.Couleur,
                    ligne.TypeCouture
                });
            _db.Execute(
                @"UPDATE Peluches SET Stock = Stock - 1 WHERE Id = @PelucheId AND Stock > 0",
                new { ligne.PelucheId });
        }
    }
    public List<Commande> GetCommandesByUser(int userId)
    {
        var commandes = _db.Query<Commande>(
            "SELECT * FROM Commandes WHERE UserId = @UserId ORDER BY DateCommande DESC",
            new { UserId = userId }).ToList();

        foreach (var cmd in commandes)
        {
            var lignes = _db.Query<LigneCommande>(
                "SELECT * FROM LigneCommande WHERE CommandeId = @CommandeId",
                new { CommandeId = cmd.Id }).ToList();

            cmd.Lignes = lignes;
        }

        return commandes;
    }

}
