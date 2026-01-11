using System.ComponentModel.DataAnnotations;

namespace MVC_Auktioner.Models;

public class SellAuctionModel
{
    [Required]
    public int Id { get; set;}

    [Required]
    public string InventoryId { get; set;} = string.Empty;

    [Required, Range(0, float.MaxValue)]
    public float FinalPrice { get; set;}

    [Required, Range(0, float.MaxValue)]
    public float StartingPrice { get; set;}
}