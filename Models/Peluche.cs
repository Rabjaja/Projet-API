using System.ComponentModel.DataAnnotations;

public class Peluche
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Description { get; set; }
    public decimal Prix { get; set; }
    public string ImageUrl { get; set; }
    public int Stock { get; set; }
}