using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UserExistsByIdAsync(int id);
    void ToggleLazyLoading(bool status = false);
}
