using HomeLedger.API.Dtos.User;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using HomeLedger.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Services.Users;

public class UserServices(HomeLedgerDbContext db) : IUserServices
{
  private readonly HomeLedgerDbContext _db = db;

  public IQueryable<User> GetUserQueryAble()
  {
    return _db.Users.AsQueryable();
  }

  public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
  {
    return await _db
        .Users.Select(u => new UserResponseDto
        {
          Id = u.UserId,
          UserName = u.UserName,
          FirstName = u.FirstName,
          LastName = u.LastName,
          Email = u.Email,
        })
        .ToListAsync();
  }

  public async Task<UserResponseDto> GetUserAsync(int id)
  {
    // check if user exists
    var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);

    if (user == null)
      return null!;

    return new UserResponseDto
    {
      UserName = user.UserName,
      Email = user.Email,
      FirstName = user.FirstName,
      LastName = user.LastName,
    };
  }

  public async Task<User> GetUserByEmailAsync(string email)
  {
    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null)
      return null!;

    return user;
  }

  public async Task<UserResponseDto> CreateUserAsync(CreateRequestDto createUser)
  {
    // check if user exists
    var email = createUser.Email?.Trim().ToLower();
    if (await _db.Users.AnyAsync(u => u.Email == email))
    {
      return null!;
    }

    var passwordHash =
        PasswordManager.Hash(createUser.Password)
        ?? throw new InvalidOperationException("Password is required.");

    var newUser = new User
    {
      UserName = createUser.UserName,
      FirstName = createUser.FirstName,
      LastName = createUser.LastName,
      Password = passwordHash,
      Email = email,
      LastLogin = DateTime.UtcNow,
      IsSuperUser = false,
    };

    await _db.Users.AddAsync(newUser);
    await _db.SaveChangesAsync();

    return new UserResponseDto
    {
      UserName = newUser.UserName,
      Email = newUser.Email,
      FirstName = newUser.FirstName,
      LastName = newUser.LastName,
    };
  }

  public async Task<bool> DeleteUserAsync(int id)
  {
    // check if user exists
    var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
      return false;
    }
    _db.Users.Remove(user);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> UpdateUserAsync(int id, UpdateRequestDto updateUser)
  {
    // check if user exists
    var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);

    if (user == null)
    {
      return false;
    }

    var hashedPassword = PasswordManager.Hash(updateUser.Password);

    user.FirstName = updateUser.FirstName;
    user.LastName = updateUser.LastName;
    user.Email = updateUser.Email;
    user.Password = hashedPassword;
    user.IsSuperUser = updateUser.IsSuperUser;

    _db.Entry(user).State = EntityState.Modified;
    await _db.SaveChangesAsync();

    return true;
  }
}
