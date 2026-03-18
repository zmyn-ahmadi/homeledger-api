using HomeLedger.API.Dtos.User;
using HomeLedger.API.Models;

namespace HomeLedger.API.Services.UserServices;

public interface IUserServices
{
  IQueryable<User> GetUserQueryAble();
  Task<IEnumerable<UserResponseDto>> GetUsersAsync();
  Task<UserResponseDto> GetUserAsync(int id);
  Task<User> GetUserByEmailAsync(string email);
  Task<UserResponseDto> CreateUserAsync(CreateRequestDto request);
  Task<bool> UpdateUserAsync(int id, UpdateRequestDto request);
  Task<bool> DeleteUserAsync(int id);
}
