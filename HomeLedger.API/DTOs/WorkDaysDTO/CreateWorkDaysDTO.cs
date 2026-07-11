using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.WorkDaysDTO;

public class CreateWorkDaysDTO
{
    [Required]
    public DateTime WeekStartDate { get; set; }

    [Required]
    [MinLength(7)]
    [MaxLength(7)]
    public bool[] DaysWorked { get; set; } = [false, false, false, false, false, false, false];
}
