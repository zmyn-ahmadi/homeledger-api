namespace HomeLedger.API.DTOs.WorkDaysDTO;

public class WorkDaysDTO
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public bool[] DaysWorked { get; set; } = [false, false, false, false, false, false, false];
}
