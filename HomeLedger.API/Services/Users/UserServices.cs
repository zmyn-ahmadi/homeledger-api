using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Utils;

namespace HomeLedger.API.Services.Users;

public class UserServices(IUnitOfWork _uow, UserMapper mapper, ILogger<UserServices> logger)
    : IUserServices
{
    private readonly UserMapper _mapper = mapper;
    private readonly ILogger<UserServices> _logger = logger;

    public IQueryable<User> GetUserQueryAble()
    {
        var users = _uow.Users.GetQueryable();
        return users;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        var users = await _uow.Users.GetAllAsync();
        return [.. users.Select(_mapper.ToDto)];
    }

    public async Task<UserDTO?> GetUserAsync(int id)
    {
        // check if user exists
        var user = await _uow.Users.GetByIdAsync(id);

        if (user is null)
        {
            _logger.LogWarning("User: {UserId} not found.", id);
            return null;
        }
        return _mapper.ToDto(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _uow.Users.GetByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("User: {Email} not found.", email);
            return null;
        }
        return user;
    }

    public async Task<UserDTO> CreateUserAsync(CreateUserDTO request)
    {
        // check if user exists
        var email = request.Email.Trim().ToLower();
        var existingUser = await _uow.Users.GetByEmailAsync(email);

        if (existingUser != null)
        {
            _logger.LogWarning("User: {Email} already exists", email);
            // InvalidOperationException surfaces as a 400 with this message, so
            // the client can show it instead of a generic 500.
            throw new InvalidOperationException($"User '{request.Email}' already exists.");
        }

        var newUser = _mapper.ToEntity(request);

        var passwordHash = PasswordManager.Hash(newUser.Password);

        newUser.Password = passwordHash;
        newUser.Email = email;
        newUser.IsActive = true;
        newUser.IsSuperUser = false;

        await _uow.Users.AddAsync(newUser);
        await _uow.CommitAsync();

        _logger.LogInformation("Created user {Email}", newUser.Email);

        return _mapper.ToDto(newUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var result = await _uow.Users.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted user {UserId}", id);
        }
        else
        {
            _logger.LogWarning("User: {UserId} not found for deletion.", id);
        }
        return result;
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO request)
    {
        // check if user exists
        var user = await _uow.Users.GetByIdAsync(id);

        if (user is null)
        {
            _logger.LogWarning("User: {UserId} not found for update.", id);
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            var exists = await _uow.Users.GetByEmailAsync(normalizedEmail);
            if (exists != null && exists.UserId != id)
            {
                _logger.LogWarning("User: user with {Email} already exists.", normalizedEmail);
                return false;
            }

            user.Email = normalizedEmail;
        }

        _mapper.UpdateEntity(request, user);

        await _uow.Users.UpdateAsync(user);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated user {Email}", user.Email);

        return true;
    }

    public void ToggleLazyLoading(bool status)
    {
        _uow.Users.ToggleLazyLoading(status);
    }
}
