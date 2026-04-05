using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.BillPaymentsDTO;

public class UpdateBillPaymentDTO
{
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
