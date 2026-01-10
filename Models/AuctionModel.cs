using System.ComponentModel.DataAnnotations;

namespace MVC_Auktioner.Models;

public class AuctionModel
{
    [Required]
    public int Id { get; set; } 

    [Required, RegularExpression(@"^[A-Za-z]{3}\d{6}$", ErrorMessage = "ID must be 3 letter followed by 6 numbers.")]
    public string InventoryId { get; set; } = string.Empty;
    
    [Required]
    public int Decade { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
    public int CategoryId { get; set; }

    public CategoryModel? Category { get; set; }

    [Required, StringLength(40)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [Required, StringLength(50)]
    public string Location { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Seller { get; set; } = string.Empty;

    [Required, Range(0, float.MaxValue)]
    public float StartingPrice { get; set; }
    
    [Required, Range(0, float.MaxValue)]
    public float PurchaseCost { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Range(0, float.MaxValue)]
    public float? FinalPrice { get; set; }

    [Required]
    public bool Sold { get; set; } = false;
}
