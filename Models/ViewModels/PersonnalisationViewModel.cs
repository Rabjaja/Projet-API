using System.ComponentModel.DataAnnotations;

public class PersonnalisationViewModel
{
    [Required]
    public int PelucheId { get; set; }

    [Display(Name = "Nom brodé")]
    [Required(ErrorMessage = "Veuillez entrer un nom à broder.")]
    [StringLength(50)]
    public string NomBrode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Veuillez choisir une couleur.")]
    public string Couleur { get; set; } = string.Empty;

    [Display(Name = "Type de couture")]
    [Required(ErrorMessage = "Veuillez sélectionner un type de couture.")]
    public string TypeCouture { get; set; } = string.Empty;
}
