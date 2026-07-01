using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class Category : BaseAuditableEntity
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Category name must be 100 characters or less.")]
    public required string Name { get; set; }

    [MaxLength(7)]
    public string Color { get; set; } = "#6366f1";

    public string? Icon { get; set; }

    // a category belongs to a user
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;
}
