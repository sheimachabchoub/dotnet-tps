using System.ComponentModel.DataAnnotations;

namespace TP6_WEBAPI.Models;

public class Operation
{
    [Key]
    public int OperationId { get; set; }

    [Required]
    [RegularExpression(
        "^(Retrait|Versement|Virement)$",
        ErrorMessage = "Le type doit être Retrait, Versement ou Virement."
    )]
    public string TypeOperation { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public double Montant { get; set; }

    public DateTime DateOperation { get; set; } = DateTime.Now;

    [Required]
    public int CompteId { get; set; }

    public Compte? Compte { get; set; }
}
