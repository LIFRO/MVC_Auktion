using System.ComponentModel.DataAnnotations;

namespace MVC_Auktioner.Models;

public class SoldEditAuctionModel
{
    [Required]
    public int Id { get; set; } 

    [Required]
    public int Decade { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
    public int CategoryId { get; set; }

    [Required, StringLength(40)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [Required, StringLength(50)]
    public string Location { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Seller { get; set; } = string.Empty;

    [Range(0, float.MaxValue)]
    public float FinalPrice { get; set; }

    [Required]
    public bool Sold { get; set; }

    [Required]
    public bool Delivered { get; set; }
}
