namespace MVC_Auktioner.Models;

public class AuctionModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Image { get; set;} = "";
    public int Year { get; set; } = ""; 
}
