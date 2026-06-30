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

    public async Task<int?> GetCurrentUserFamilyIdAsync()
    {
        var userId = _userCtx.UserId;
        var membership = await _uow.FamilyMembers.GetByUserIdAsync(userId);
        return membership?.FamilyId;
    }

    public async Task<FamilyDTO?> GetByIdAsync(int id)
    {
        var familyId = await GetCurrentUserFamilyIdAsync();

        if (familyId != id)
        {
            _logger.LogWarning(
                "User {UserId} attempted to access Family {FamilyId} -- denied.",
                _userCtx.UserId,
                id
            );

            return null;
        }

        var fam = await _uow.Families.GetByIdAsync(id);

        if (fam is null)
        {
            _logger.LogWarning("Family: {FamilyId} not found", id);
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

        var member = new FamilyMember
        {
            UserId = user.UserId,
            FamilyId = newFam.FamilyId,
            Role = "Owner",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        await _uow.FamilyMembers.AddAsync(member);
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

        // 2. build a Link
        var familyId =
            await GetCurrentUserFamilyIdAsync()
            ?? throw new InvalidOperationException("User is not a member of any family");
        // 1. Generate code
        var code = GenerateCode();

        var link = new Models.Link
        {
            Code = code,
            FamilyId = familyId,
            CreatedBy = userId,
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            MaxUses = 1,
            UseCount = 0,
        };

        await _uow.Links.AddAsync(link);
        await _uow.CommitAsync();

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

        if (invitation.FamilyId is null)
            throw new InvalidOperationException("Invitation is not linked to a family.");

        var invitedFamilyId = invitation.FamilyId.Value;

        var existingMembership = await _uow.FamilyMembers.GetByUserIdAndFamilyIdAsync(
            userId,
            invitedFamilyId
        );

        if (existingMembership is not null)
            return;

        var member = new FamilyMember
        {
            UserId = userId,
            FamilyId = invitedFamilyId,
            Role = "Member",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        await _uow.FamilyMembers.AddAsync(member);

        invitation.UseCount += 1;

        await _uow.CommitAsync();
    }

    // Bill
    public async Task<List<BillDTO?>> GetFamilyBillsAsync()
    {
        var familyId = await GetCurrentUserFamilyIdAsync();

        if (familyId is null)
        {
            _logger.LogWarning("User {UserId} is not in any family.", _userCtx.UserId);
            return [];
        }

        var bills = await _uow.Families.GetFamilyBillsAsync(familyId.Value);

        return [.. bills.Select(_billMapper.ToDto)];
    }

    public async Task<BillDTO?> GetFamilyBillAsync(int id)
    {
        var familyId = await GetCurrentUserFamilyIdAsync();

        if (familyId is null)
        {
            _logger.LogWarning("User {UserId} is not in any family.", _userCtx.UserId);
            return null;
        }
        var bill = await _uow.Families.GetFamilyBillAsync(familyId.Value, id);
        if (bill is null)
        {
            _logger.LogWarning(
                "Bill {BillId} is not present in the family group {FamilyId}",
                id,
                familyId
            );
            return null;
        }
        return _billMapper.ToDto(bill);
    }

    public async Task<BillDTO> AddFamilyBillAsync(CreateBillDTO req)
    {
        var familyId =
            await GetCurrentUserFamilyIdAsync()
            ?? throw new InvalidOperationException("User is not found in any family");

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["FamilyId"] = familyId }
        );

        var newBill = _billMapper.ToEntity(req);
        newBill.FamilyId = familyId;
        newBill.Scope = "Family";
        newBill.CreatedAt = DateTime.UtcNow;
        newBill.IsActive = true;

        await _uow.Bills.AddAsync(newBill);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Bill {BillId} for family {FamilyId}",
            newBill.BillId,
            newBill.FamilyId
        );

        return _billMapper.ToDto(newBill);
    }

    // Budget
    public async Task<List<BudgetDTO?>> GetFamilyBudgetsAsync()
    {
        var familyId = await GetCurrentUserFamilyIdAsync();

        if (familyId is null)
        {
            _logger.LogWarning("User {UserId} is not in any family.", _userCtx.UserId);
            return [];
        }

        var budgets = await _uow.Families.GetFamilyBudgetsAsync(familyId.Value);

        return [.. budgets.Select(_budgetMapper.ToDto)];
    }

    public async Task<BudgetDTO?> GetFamilyBudgetAsync(int id)
    {
        var familyId = await GetCurrentUserFamilyIdAsync();

        if (familyId is null)
        {
            _logger.LogWarning("User {UserId} is not in any family.", _userCtx.UserId);
            return null;
        }

        var budget = await _uow.Families.GetFamilyBudgetAsync(familyId.Value, id);
        if (budget is null)
        {
            _logger.LogWarning(
                "Budget {BudgetId} is not present in the family group {FamilyId}",
                id,
                familyId
            );
            return null;
        }
        return _budgetMapper.ToDto(budget);
    }

    public async Task<BudgetDTO> AddFamilyBudgetAsync(CreateBudgetDTO req)
    {
        var familyId =
            await GetCurrentUserFamilyIdAsync()
            ?? throw new InvalidOperationException("User not found in any family");

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["FamilyId"] = familyId }
        );

        var newBudget = _budgetMapper.ToEntity(req);
        newBudget.FamilyId = familyId;
        newBudget.CreatedAt = DateTime.UtcNow;
        newBudget.IsActive = true;

        await _uow.Budgets.AddAsync(newBudget);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created budget {BudgetId} for family {FamilyId}",
            newBudget.BudgetId,
            newBudget.FamilyId
        );

        return _budgetMapper.ToDto(newBudget);
    }

    // Expense
    public async Task<List<ExpenseDTO>> GetFamilyExpensesAsync()
    {
        var familyId = await GetCurrentUserFamilyIdAsync();

        if (familyId is null)
        {
            _logger.LogWarning("User {UserId} is not in any family.", _userCtx.UserId);
            return [];
        }

        var expenses = await _uow.Families.GetFamilyExpensesAsync(familyId.Value);

        return [.. expenses.Select(_expenseMapper.ToDto)];
    }

    public async Task<ExpenseDTO> AddFamilyExpenseAsync(CreateExpenseDTO req)
    {
        // Get Member's Expense in the Family
        throw new NotImplementedException();
    }

    // Income
    public async Task<List<IncomeDTO>> GetFamilyIncomesAsync()
    {
        var familyId = await GetCurrentUserFamilyIdAsync();
        if (familyId is null)
        {
            _logger.LogWarning("User {UserId} is not in any family", _userCtx.UserId);
            return [];
        }

        var incomes = await _uow.Families.GetFamilyIncomesAsync(familyId.Value);
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
