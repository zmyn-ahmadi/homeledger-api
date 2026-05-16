using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HomeLedger.API.DTOs.BillPaymentsDTO;

public class BillPaymentDTO
{
    [JsonIgnore]
    public int BillPaymentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidDate { get; set; }
    public string? Notes { get; set; }
    public BillDTO.BillDTO Bill { get; set; }
    public UserDTO.UserDTO PaidBy { get; set; }
}
