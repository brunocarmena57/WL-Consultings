using WLConsultingChallenge.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using WLConsultingChallenge.Core.Entities;
using System.Security.Cryptography;
using DevOne.Security.Cryptography.BCrypt;
using WLConsultingChallenge.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace WLConsultingChallenge.Core.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public UserService(AppDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<User> RegisterUser(string username, string email, string password)
    {
        var hash = BCryptHelper.GenerateSalt(12);
        var passwordHash = _authService.HashPassword(password, hash);
            
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create wallet for the user
        var wallet = new Wallet
        {
            UserId = user.Id,
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}