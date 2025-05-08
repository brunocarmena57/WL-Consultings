using WLConsultingChallenge.Core.Entities;

namespace WLConsultingChallenge.Core.Services;

public interface IUserService
{
    Task<User> RegisterUser(string username, string email, string password);
    Task<User> GetUserByUsername(string username);
    Task<User> GetUserById(int id);
}