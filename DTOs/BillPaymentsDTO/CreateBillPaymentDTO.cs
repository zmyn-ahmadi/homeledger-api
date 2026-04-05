using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HomeLedger.API.DTOs.BillPaymentsDTO;

public class CreateBillPaymentDTO
{
    [Key]
    [JsonIgnore]
    public int BillPaymentId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be between 2 and 10")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaidDate { get; set; }
    public string? Notes { get; set; }

    [Key]
    [Required]
    public int BillId { get; set; }

    [Key]
    [Required]
    public int PaidById { get; set; }
}
