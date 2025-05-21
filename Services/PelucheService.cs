using Dapper;
using Npgsql;
using System.Data;

namespace PeluShop.Services;

public class PelucheService : IPelucheService
{
    private readonly string? _connectionString;

    public PelucheService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    private IDbConnection Connection => new NpgsqlConnection(_connectionString);

    public List<Peluche> GetAll()
    {
        using var db = Connection;
        var sql = "SELECT * FROM Peluches WHERE Stock > 0 ORDER BY id";
        return db.Query<Peluche>(sql).ToList();
    }

    public Peluche? GetById(int id)
    {
        using var db = Connection;
        var sql = "SELECT * FROM Peluches WHERE Id = @Id";
        return db.QueryFirstOrDefault<Peluche>(sql, new { Id = id });
    }

    public void Add(Peluche peluche)
    {
        using var db = Connection;
        var sql = @"
            INSERT INTO Peluches (Nom, Prix, Description, ImageUrl, Stock)
            VALUES (@Nom, @Prix, @Description, @ImageUrl, @Stock)";
        db.Execute(sql, peluche);
    }

    public void Update(Peluche peluche)
    {
        using var db = Connection;
        var sql = @"
            UPDATE Peluches
            SET Nom = @Nom, Prix = @Prix, Description = @Description, ImageUrl = @ImageUrl, Stock = @Stock
            WHERE Id = @Id";
        db.Execute(sql, peluche);
    }

    public bool Delete(int id)
    {
        using var db = Connection;
        var sqlCheck = "SELECT COUNT(*) FROM lignecommande WHERE pelucheid = @Id";
        int count = db.ExecuteScalar<int>(sqlCheck, new { Id = id });

        if (count == 0)
        {
            var sqlDelete = "DELETE FROM Peluches WHERE Id = @Id";
            db.Execute(sqlDelete, new { Id = id });
            return true;
        }

        return false;
    }

}
