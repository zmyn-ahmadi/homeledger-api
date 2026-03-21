using HomeLedger.API.Dtos.User;
using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Users;

public interface IUserServices
{
  IQueryable<User> GetUserQueryAble();
  Task<IEnumerable<User>> GetUsersAsync();
  Task<User?> GetUserAsync(int id);
  Task<User?> GetUserByEmailAsync(string email);
  Task<User> CreateUserAsync(CreateRequestDto request);
  Task<bool> UpdateUserAsync(int id, UpdateRequestDto request);
  Task<bool> DeleteUserAsync(int id);
}
