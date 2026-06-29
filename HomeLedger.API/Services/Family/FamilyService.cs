using System.Security.Cryptography;
using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.DTOs.BudgetDTO;
using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Utils.UserContext;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeLedger.API.Services.Family;

public class FamilyService(
    IUnitOfWork _uow,
    FamilyMapper mapper,
    BillMapper billMapper,
    BudgetMapper budgetMapper,
    ExpenseMapper expenseMapper,
    IncomeMapper incomeMapper,
    ILogger<FamilyService> logger,
    IConfiguration config,
    IUserContext userContext
) : IFamilyService
{
    private readonly IConfiguration _config = config;
    private readonly ILogger<FamilyService> _logger = logger;
    private readonly FamilyMapper _mapper = mapper;
    private readonly BillMapper _billMapper = billMapper;
    private readonly BudgetMapper _budgetMapper = budgetMapper;
    private readonly ExpenseMapper _expenseMapper = expenseMapper;
    private readonly IncomeMapper _incomeMapper = incomeMapper;
    private readonly IUserContext _userCtx = userContext;

    public async Task<FamilyDTO?> GetByIdAsync(int id)
    {
        // get userId from jwt
        var userId = _userCtx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        // get user
        var user = await _uow.Users.GetByIdAsync(userId) ?? throw new Exception("User not found.");

        var callerFamilyId = user.FamilyId;
        if (callerFamilyId != id)
        {
            _logger.LogWarning(
                "User {UserId} attempted t o access Family {FamilyId} -- denied.",
                _userCtx.UserId,
                id
            );
            return null;
        }

        var fam = await _uow.Families.GetByIdAsync(id);
        if (fam is null)
        {
            _logger.LogWarning("Family: {FamilyId} not found.", id);
            return null;
        }

        return _mapper.ToDto(fam);
    }

    public async Task<IEnumerable<FamilyDTO>> GetAllAsync()
    {
        var fams = await _uow.Families.GetAllAsync();
        return [.. fams.Select(_mapper.ToDto)];
    }

    public async Task<FamilyDTO> AddAsync(CreateFamilyDTO request)
    {
        // get userId from jwt
        var userId = _userCtx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        // get user
        var user = await _uow.Users.GetByIdAsync(userId) ?? throw new Exception("User not found.");

        // map
        var newFam = _mapper.ToEntity(request);

        // assign fields
        newFam.CreatedAt = DateTime.UtcNow;
        newFam.IsActive = true;

        await _uow.Families.AddAsync(newFam);
        await _uow.CommitAsync();

        user.Family = newFam;
        user.FamilyId = newFam.FamilyId;

        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Family {FamilyId} by user {OwnerId}",
            newFam.FamilyId,
            userId
        );

        return _mapper.ToDto(newFam);
    }

    public async Task<FamilyDTO?> UpdateAsync(int id, UpdateFamilyDTO request)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["FamilyId"] = id });

        var existingFam = await _uow.Families.GetByIdAsync(id);
        if (existingFam is null)
        {
            _logger.LogWarning("Family: {FamilyId} not found for update", id);
            return null;
        }
        _mapper.UpdateEntity(request, existingFam);
        existingFam.UpdatedAt = DateTime.UtcNow;

        await _uow.Families.UpdateAsync(existingFam);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated Family {FamilyId}", id);

        return _mapper.ToDto(existingFam);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["FamilyId"] = id });

        var result = await _uow.Families.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Family {FamilyId}", id);
        }
        else
        {
            _logger.LogWarning("Family: {FamilyId} not found for deletion", id);
        }

        return result;
    }

    public async Task<string> InviteAsync()
    {
        var userId = _userCtx.UserId;
        var user =
            await _uow.Users.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found");
        if (user.FamilyId is null)
            throw new InvalidOperationException("User is not a member of any family");

        // 1. Generate code
        var code = GenerateCode();

        // 2. build a Link
        var link = new Models.Link
        {
            Code = code,
            FamilyId = user.FamilyId.Value,
            CreatedBy = user.UserId,
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            MaxUses = 1,
            UseCount = 0,
        };

        // 3. add to database
        await _uow.Links.AddAsync(link);
        await _uow.CommitAsync();

        // 3. return the generated link
        return $"{_config["App:BaseUrl"]}/invite?code={code}";
    }

    public async Task AcceptInvitationAsync(string code)
    {
        // 1. Grab the link for the database
        var invitation =
            await _uow.Links.GetLinkByCode(code)
            ?? throw new InvalidOperationException("Invitation not found.");

        if (invitation.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Invitation has expired.");

        if (invitation.IsRevoked)
            throw new InvalidOperationException("Invitation has been revoked");

        if (invitation.MaxUses.HasValue && invitation.UseCount >= invitation.MaxUses.Value)
            throw new InvalidOperationException("Invitation has reached its use limit.");

        var userId = _userCtx.UserId;
        var user =
            await _uow.Users.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found");

        if (user.FamilyId is not null)
        {
            if (user.FamilyId == invitation.FamilyId)
                return;
            throw new InvalidOperationException("User is already a member of a family.");
        }

        user.FamilyId = invitation.FamilyId;
        invitation.UseCount += 1;

        await _uow.CommitAsync();
    }

    // Bill
    public async Task<List<BillDTO?>> GetFamilyBillsAsync()
    {
        // 1. Get userId
        var userId = _userCtx.UserId;

        // 2. Get User
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user is null)
        {
            InvalidOperationException invalidOperationException = new("User not found.");
            throw invalidOperationException;
        }

        // 3. Check if user has family
        if (user.Family is null)
        {
            _logger.LogWarning(
                "User: {userId} is not in family: {FamilyId}",
                userId,
                user.FamilyId
            );
            return [];
        }

        // 4. if so, get their bills
        var bills = await _uow.Families.GetFamilyBillsAsync(user.FamilyId.Value);
        return [.. bills.Select(_billMapper.ToDto)];
    }

    public async Task<BillDTO> AddFamilyBillAsync(CreateBillDTO req)
    {
        throw new NotImplementedException();
    }

    // Budget
    public async Task<List<BudgetDTO>> GetFamilyBudgetsAsync()
    {
        // 1. Get userId
        var userId = _userCtx.UserId;

        // 2. Get User
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user is null)
        {
            InvalidOperationException invalidOperationException = new("User not found.");
            throw invalidOperationException;
        }

        // 3. Check if user has family
        if (user.Family is null)
        {
            _logger.LogWarning(
                "User: {userId} is not in family: {FamilyId}",
                userId,
                user.FamilyId
            );
            return [];
        }

        var budgets = await _uow.Families.GetFamilyBudgetsAsync(user.FamilyId.Value);
        return [.. budgets.Select(_budgetMapper.ToDto)];
    }

    public async Task<BudgetDTO> AddFamilyBudgetAsync(CreateBudgetDTO req)
    {
        throw new NotImplementedException();
    }

    // Expense
    public async Task<List<ExpenseDTO>> GetFamilyExpensesAsync()
    {
        // 1. Get userId
        var userId = _userCtx.UserId;

        // 2. Get User
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user is null)
        {
            InvalidOperationException invalidOperationException = new("User not found.");
            throw invalidOperationException;
        }

        // 3. Check if user has family
        if (user.Family is null)
        {
            _logger.LogWarning(
                "User: {userId} is not in family: {FamilyId}",
                userId,
                user.FamilyId
            );
            return [];
        }

        var expenses = await _uow.Families.GetFamilyExpensesAsync(user.FamilyId.Value);
        return [.. expenses.Select(_expenseMapper.ToDto)];
    }

    public async Task<ExpenseDTO> AddFamilyExpenseAsync(CreateExpenseDTO req)
    {
        throw new NotImplementedException();
    }

    // Income
    public async Task<List<IncomeDTO>> GetFamilyIncomesAsync()
    {
        // 1. Get userId
        var userId = _userCtx.UserId;

        // 2. Get User
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user is null)
        {
            InvalidOperationException invalidOperationException = new("User not found.");
            throw invalidOperationException;
        }

        // 3. Check if user has family
        if (user.Family is null)
        {
            _logger.LogWarning(
                "User: {userId} is not in family: {FamilyId}",
                userId,
                user.FamilyId
            );
            return [];
        }

        var incomes = await _uow.Families.GetFamilyIncomesAsync(user.FamilyId.Value);
        return [.. incomes.Select(_incomeMapper.ToDto)];
    }

    public async Task<IncomeDTO> AddFamilyIncomeAsync(CreateIncomeDTO req)
    {
        throw new NotImplementedException();
    }

    private string GenerateCode()
    {
        return Convert
            .ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
