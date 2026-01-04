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

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;


    [Required, StringLength(40)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [Required, Range(0, double.MaxValue)]
    public decimal StartingPrice { get; set; }
    
    [Required, Range(0, double.MaxValue)]
    public decimal PurchaseCost { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal? FinalPrice { get; set; }

    [Required]
    public bool Sold { get; set; } = false;
}
