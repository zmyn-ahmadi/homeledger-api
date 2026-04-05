using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Utils;

namespace HomeLedger.API.Services.Users;

public class UserServices(IUnitOfWork uow, UserMapper mapper) : IUserServices
{
    private readonly UserMapper _mapper = mapper;

    public IQueryable<User> GetUserQueryAble()
    {
        var users = uow.Users.GetQueryable();
        return users;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        var users = await uow.Users.GetAllAsync();
        return users.Select(_mapper.ToDto);
    }

    public async Task<UserDTO> GetUserAsync(int id)
    {
        // check if user exists
        var user =
            await uow.Users.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id:{id} not found.");
        return _mapper.ToDto(user);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await uow.Users.GetByEmailAsync(email);
    }

    public async Task<UserDTO> CreateUserAsync(CreateUserDTO request)
    {
        // check if user exists
        var email = request.Email.Trim().ToLower();
        var existingUser = await uow.Users.GetByEmailAsync(email);

        if (existingUser != null)
            throw new Exception($"User '{request.Email}' already exists.");

        var newUser = _mapper.ToEntity(request);

        var passwordHash = PasswordManager.Hash(newUser.Password);

        newUser.Password = passwordHash;
        newUser.Email = email;
        newUser.IsActive = true;
        newUser.IsSuperUser = false;

        await uow.Users.AddAsync(newUser);
        await uow.CommitAsync();

        return _mapper.ToDto(newUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var result = await uow.Users.DeleteAsync(id);
        if (result)
            await uow.CommitAsync();
        return result;
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO request)
    {
        // check if user exists
        var user =
            await uow.Users.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id:{id} not found.");

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            var exists = await uow.Users.GetByEmailAsync(normalizedEmail);
            if (exists != null && exists.UserId != id)
                throw new InvalidOperationException("Email already in use.");

            user.Email = normalizedEmail;
        }

        _mapper.UpdateEntity(request, user);

        await uow.Users.UpdateAsync(user);
        await uow.CommitAsync();
        return true;
    }

    public void ToggleLazyLoading(bool status)
    {
        uow.Users.ToggleLazyLoading(status);
    }
}
