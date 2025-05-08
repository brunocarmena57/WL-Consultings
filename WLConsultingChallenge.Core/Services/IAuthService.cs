using WLConsultingChallenge.Core.Entities;

namespace WLConsultingChallenge.Core.Services;

public interface IAuthService
{
        string GenerateJwtToken(User user);
        string HashPassword(string password, string hash);
        bool VerifyPassword(string password, string hash);
}