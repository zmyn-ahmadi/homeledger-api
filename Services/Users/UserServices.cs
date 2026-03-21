using HomeLedger.API.Dtos.User;
using HomeLedger.API.Models;
using HomeLedger.API.Utils;

namespace HomeLedger.API.Services.Users;

public class UserServices(IUnitOfWork uow) : IUserServices
{
    public IQueryable<User> GetUserQueryAble()
    {
        return uow.Users.GetQueryable();
    }

    public Task<IEnumerable<User>> GetUsersAsync()
    {
        return uow.Users.GetAllAsync();
    }

    public async Task<User?> GetUserAsync(int id)
    {
        // check if user exists
        return await uow.Users.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id:{id} not found.");
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return uow.Users.GetByEmailAsync(email);
    }

    public async Task<User> CreateUserAsync(CreateRequestDto createUser)
    {
        // check if user exists
        var email = createUser.Email?.Trim().ToLower();
        var existingUser = await uow.Users.GetByEmailAsync(email);

        if (existingUser != null)
            throw new InvalidOperationException($"User '{createUser.Email}' already exists.");

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

        await uow.Users.AddAsync(newUser);
        await uow.CommitAsync();

        return newUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var result = await uow.Users.DeleteAsync(id);
        if (result)
            await uow.CommitAsync();
        return result;
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateRequestDto updateUser)
    {
        // check if user exists
        var user =
            await uow.Users.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id:{id} not found.");

        if (!string.IsNullOrWhiteSpace(updateUser.Password))
            user.Password = PasswordManager.Hash(updateUser.Password);

        user.FirstName = updateUser.FirstName;
        user.LastName = updateUser.LastName;
        user.Email = updateUser.Email?.Trim().ToLower();
        user.IsSuperUser = updateUser.IsSuperUser;

        await uow.Users.UpdateAsync(user);
        await uow.CommitAsync();
        return true;
    }
}
