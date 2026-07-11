using HomeLedger.API.DTOs.WorkDaysDTO;
using HomeLedger.API.Services.WorkDays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class WorkDaysController(IWorkDaysService workDaysService) : ControllerBase
{
    private readonly IWorkDaysService _service = workDaysService;

    [HttpGet]
    public async Task<ActionResult<WorkDaysDTO>> GetWorkDaysByWeek(
        [FromQuery] DateTime weekStartDate
    )
    {
        var workDays = await _service.GetByWeekAsync(weekStartDate);
        return workDays is null ? NoContent() : Ok(workDays);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkDaysDTO>> GetWorkDays(int id)
    {
        var workDays = await _service.GetByIdAsync(id);
        if (workDays == null)
            return NotFound(new { Message = $"WorkDays with ID {id} not found." });

        return Ok(workDays);
    }

    [HttpPost]
    public async Task<ActionResult<WorkDaysDTO>> PostWorkDays(
        [FromBody] CreateWorkDaysDTO createWorkDays
    )
    {
        if (createWorkDays == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var workDays = await _service.SaveWeekAsync(createWorkDays);
        return CreatedAtAction(nameof(GetWorkDays), new { id = workDays.Id }, workDays);
    }
}
