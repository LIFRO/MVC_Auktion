using System.ComponentModel.DataAnnotations;

namespace MVC_Auktioner.Models;

public class AuctionModel
{
    [Required]
    public string Id { get; set; } = string.empty;

    [Required]
    [StringLenght(40)]
    public string Title { get; set;} = string.Empty;

    [StringLength(200)]
    public string? description { get; set;}

    [Required]
    [Range(0, double.MaxValue)]
    public decimal StartingPrice { get; set; }

    public DateTune CreationDate { get; set; } = DateTime.UtcNow;
}
