using System.ComponentModel.DataAnnotations;
using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.CategoryDTO;

public class CreateCategoryDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Color { get; set; }
    public string? Icon { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    public TransactionType Type { get; set; }
}
