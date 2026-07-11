using HomeLedger.API.DTOs.WorkDaysDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Utils.UserContext;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Services.WorkDays;

public class WorkDaysService(
    IUnitOfWork _uow,
    WorkDaysMapper mapper,
    ILogger<WorkDaysService> logger,
    IUserContext userContext
) : IWorkDaysService
{
    private readonly WorkDaysMapper _mapper = mapper;
    private readonly ILogger<WorkDaysService> _logger = logger;
    private readonly IUserContext _userContext = userContext;

    public async Task<WorkDaysDTO?> GetByIdAsync(int id)
    {
        var workDays = await _uow.Repository<Models.WorkDays>().GetByIdAsync(id);
        if (workDays is null)
        {
            _logger.LogWarning("WorkDays: {WorkDaysId} not found.", id);
            return null;
        }

        return _mapper.ToDto(workDays);
    }

    public async Task<WorkDaysDTO?> GetByWeekAsync(DateTime weekStartDate)
    {
        var userId = _userContext.UserId;
        var week = NormalizeWeekStart(weekStartDate);

        var workDays = await _uow.Repository<Models.WorkDays>()
            .GetQueryable()
            .FirstOrDefaultAsync(w => w.OwnerId == userId && w.WeekStartDate == week);

        return workDays is null ? null : _mapper.ToDto(workDays);
    }

    public async Task<WorkDaysDTO> SaveWeekAsync(CreateWorkDaysDTO request)
    {
        var userId = _userContext.UserId;
        var week = NormalizeWeekStart(request.WeekStartDate);

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var repo = _uow.Repository<Models.WorkDays>();
        var existing = await repo.GetQueryable()
            .FirstOrDefaultAsync(w => w.OwnerId == userId && w.WeekStartDate == week);

        if (existing is not null)
        {
            existing.DaysWorked = request.DaysWorked;
            await repo.UpdateAsync(existing);
            await _uow.CommitAsync();

            _logger.LogInformation(
                "Updated WorkDays {WorkDaysId} for week {WeekStartDate}",
                existing.Id,
                week
            );

            return _mapper.ToDto(existing);
        }

        var newWorkDays = new Models.WorkDays
        {
            OwnerId = userId,
            WeekStartDate = week,
            DaysWorked = request.DaysWorked,
        };

        await repo.AddAsync(newWorkDays);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created WorkDays {WorkDaysId} for week {WeekStartDate}",
            newWorkDays.Id,
            week
        );

        return _mapper.ToDto(newWorkDays);
    }

    // Npgsql requires UTC kind for timestamptz; also strip the time part so a
    // week is always keyed by its start date.
    private static DateTime NormalizeWeekStart(DateTime weekStartDate) =>
        DateTime.SpecifyKind(weekStartDate.Date, DateTimeKind.Utc);
}
