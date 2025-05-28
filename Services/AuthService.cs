using Dapper;
using Npgsql;
using PeluShop.ViewModels;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PeluShop.Services;

public class AuthService : IAuthService
{
    private readonly string ? _connectionString;

    public AuthService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    private IDbConnection Connection => new NpgsqlConnection(_connectionString);

    public async Task<bool> RegisterUserAsync(RegisterViewModel model)
    {
        using var db = Connection;

        var existingUser = await db.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username OR Email = @Email",
            new { model.Username, model.Email });

        if (existingUser != null)
            return false;

        var hashedPassword = HashPassword(model.Password);

        await db.ExecuteAsync(
            "INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES (@Username, @Email, @PasswordHash, 'User')",
            new { model.Username, model.Email, PasswordHash = hashedPassword });

        return true;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        using var db = Connection;

        var user = await db.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username",
            new { Username = username });

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new (ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "login");
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(principal);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == storedHash;
    }
    public List<User> GetAllUsers()
    {
        using var db = Connection;
        var sql = "SELECT * FROM Users";
        return db.Query<User>(sql).ToList();
    }
    public bool DeleteUser(int id)
    {
        using var db = Connection;
        string role = db.QuerySingle<string>(
            "SELECT role FROM users WHERE Id = @Id",
            new { Id = id });

        if (role != "Admin")
        {
            var sqlDelete = "DELETE FROM Users WHERE Id = @Id";
            db.Execute(sqlDelete, new { Id = id });
            return true;
        }

        return false;
    }

}
