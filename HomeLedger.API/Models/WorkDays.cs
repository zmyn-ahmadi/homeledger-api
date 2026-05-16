namespace HomeLedger.API.Models;

public class WorkDays
{
    public int Id { get; set; }
    public required int OwnerId { get; set; }
    public DateTime WeekStartDate { get; set; }

    // days worked
    // [0,0,0,0,0,0,0]
    public required bool[] DaysWorked { get; set; } =
    [false, false, false, false, false, false, false];
}
