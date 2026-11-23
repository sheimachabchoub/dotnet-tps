using System.ComponentModel.DataAnnotations;

namespace TP6_WEBAPI.Models;

public class Compte
{
    [Key]
    public int CompteId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int NumeroCompte { get; set; }

    [Range(0, double.MaxValue)]
    public double Solde { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;

    [Required]
    public int ClientId { get; set; }

    public Client? Client { get; set; }

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
}
