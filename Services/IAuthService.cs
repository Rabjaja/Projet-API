using PeluShop.ViewModels;
using System.Security.Claims;

namespace PeluShop.Services;

public interface IAuthService
{
    Task<bool> RegisterUserAsync(RegisterViewModel model);
    Task<User?> AuthenticateAsync(string username, string password);
    Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(User user);
    List<User> GetAllUsers();
    public bool DeleteUser(int id);
}
