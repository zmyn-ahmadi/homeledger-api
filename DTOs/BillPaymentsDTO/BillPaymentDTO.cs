using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.BillPaymentsDTO;

public class BillPaymentDTO
{
    [JsonIgnore]
    [Key]
    public int BillPaymentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidDate { get; set; }
    public string? Notes { get; set; }
    public virtual Bill Bill { get; set; } = null!;
    public virtual User PaidBy { get; set; } = null!;
}
