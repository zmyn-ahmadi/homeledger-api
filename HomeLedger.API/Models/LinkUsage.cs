using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class LinkUsage
{
    public int LinkUsageId { get; set; }

    public int LinkId { get; set; }
    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual Link Link { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
