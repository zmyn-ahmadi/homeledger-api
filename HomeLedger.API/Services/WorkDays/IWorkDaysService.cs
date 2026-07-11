using HomeLedger.API.DTOs.WorkDaysDTO;

namespace HomeLedger.API.Services.WorkDays;

public interface IWorkDaysService
{
    Task<WorkDaysDTO?> GetByIdAsync(int id);
    Task<WorkDaysDTO?> GetByWeekAsync(DateTime weekStartDate);
    Task<WorkDaysDTO> SaveWeekAsync(CreateWorkDaysDTO request);
}
