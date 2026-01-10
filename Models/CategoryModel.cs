using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Auktioner.Models;

public class CategoryModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}

    [Required, StringLength(30)]
    public string CategoryName { get; set;} = string.Empty;
}
