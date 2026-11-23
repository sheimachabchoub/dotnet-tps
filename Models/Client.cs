using System.ComponentModel.DataAnnotations;

namespace TP6_WEBAPI.Models;

public class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Adresse { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public ICollection<Compte> Comptes { get; set; } = new List<Compte>();
}
