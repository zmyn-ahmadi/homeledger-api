using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Users;

public interface IUserServices
{
    IQueryable<User> GetUserQueryAble();
    Task<IEnumerable<UserDTO>> GetUsersAsync();
    Task<UserDTO?> GetUserAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<UserDTO> CreateUserAsync(CreateUserDTO request);
    Task<bool> UpdateUserAsync(int id, UpdateUserDTO request);
    Task<bool> DeleteUserAsync(int id);
    void ToggleLazyLoading(bool status);
}
