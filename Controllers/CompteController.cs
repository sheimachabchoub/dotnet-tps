using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP6_WEBAPI.Data;
using TP6_WEBAPI.Models;
using TP6_WEBAPI.Repositories;

namespace TP6_WEBAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompteController : ControllerBase
{
    private readonly ICompteRepository _repository;
    private readonly AppDbContext _context;

    public CompteController(
        ICompteRepository repository,
        AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Compte>>> GetComptes()
    {
        var comptes = await _repository.GetAll();
        return Ok(comptes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Compte>> GetCompte(int id)
    {
        var compte = await _repository.GetById(id);

        if (compte is null)
        {
            return NotFound("Compte introuvable.");
        }

        return Ok(compte);
    }

    [HttpPost]
    public async Task<ActionResult<Compte>> CreateCompte(Compte compte)
    {
        var clientExiste = await _context.Clients
            .AnyAsync(c => c.ClientId == compte.ClientId);

        if (!clientExiste)
        {
            return BadRequest("Le client indiqué n'existe pas.");
        }

        var compteExiste = await _context.Comptes
            .AnyAsync(c => c.NumeroCompte == compte.NumeroCompte);

        if (compteExiste)
        {
            return BadRequest("Ce numéro de compte existe déjà.");
        }

        compte.CompteId = 0;
        compte.DateCreation = DateTime.Now;

        var nouveauCompte = await _repository.Add(compte);

        return CreatedAtAction(
            nameof(GetCompte),
            new { id = nouveauCompte.CompteId },
            nouveauCompte);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Compte>> UpdateCompte(
        int id,
        Compte compte)
    {
        if (id != compte.CompteId)
        {
            return BadRequest("L'identifiant de l'URL ne correspond pas au compte.");
        }

        var clientExiste = await _context.Clients
            .AnyAsync(c => c.ClientId == compte.ClientId);

        if (!clientExiste)
        {
            return BadRequest("Le client indiqué n'existe pas.");
        }

        var compteModifie = await _repository.Update(compte);

        if (compteModifie is null)
        {
            return NotFound("Compte introuvable.");
        }

        return Ok(compteModifie);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Compte>> DeleteCompte(int id)
    {
        try
        {
            var compteSupprime = await _repository.Delete(id);

            if (compteSupprime is null)
            {
                return NotFound("Compte introuvable.");
            }

            return Ok(compteSupprime);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("search/{clientId:int}")]
    public async Task<ActionResult<IEnumerable<Compte>>> Search(int clientId)
    {
        var comptes = await _repository.Search(clientId);
        return Ok(comptes);
    }

    [HttpPost("operation")]
    public async Task<ActionResult<Operation>> CreateOperation(Operation operation)
    {
        var compte = await _context.Comptes
            .FirstOrDefaultAsync(c => c.CompteId == operation.CompteId);

        if (compte is null)
        {
            return BadRequest("Le compte indiqué n'existe pas.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var compteMisAJour = await _repository.MajSolde(
                compte.NumeroCompte,
                operation);

            if (compteMisAJour is null)
            {
                return BadRequest("Le compte indiqué n'existe pas.");
            }

            operation.OperationId = 0;
            operation.DateOperation = DateTime.Now;

            var nouvelleOperation = await _repository.Add(operation);
            await transaction.CommitAsync();

            return Ok(nouvelleOperation);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{num:int}/nombre-operations")]
    public async Task<ActionResult<int>> NombreOperations(int num)
    {
        var compteExiste = await _context.Comptes
            .AnyAsync(c => c.NumeroCompte == num);

        if (!compteExiste)
        {
            return NotFound("Compte introuvable.");
        }

        var nombre = await _repository.NombreOperations(num);
        return Ok(nombre);
    }

    [HttpGet("{num:int}/extrait")]
    public async Task<ActionResult<IEnumerable<Operation>>> Extrait(
        int num,
        [FromQuery] DateTime date)
    {
        var operations = await _repository.ExtraitCompte(num, date);
        return Ok(operations);
    }

    [HttpGet("solde-total/{clientId:int}")]
    public async Task<ActionResult<double>> SoldeTotal(int clientId)
    {
        var clientExiste = await _context.Clients
            .AnyAsync(c => c.ClientId == clientId);

        if (!clientExiste)
        {
            return NotFound("Client introuvable.");
        }

        var solde = await _repository.SoldeTotal(clientId);
        return Ok(solde);
    }

    [HttpPost("virement")]
    public async Task<ActionResult> MajVirement(
        [FromQuery] int num1,
        [FromQuery] int num2,
        [FromQuery] double montant)
    {
        try
        {
            await _repository.MajVirement(num1, num2, montant);
            return Ok("Virement effectué avec succès.");
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("total-retraits/{clientId:int}")]
    public async Task<ActionResult<double>> TotalRetraits(int clientId)
    {
        var clientExiste = await _context.Clients
            .AnyAsync(c => c.ClientId == clientId);

        if (!clientExiste)
        {
            return NotFound("Client introuvable.");
        }

        var total = await _repository.TotalRetraits(clientId);
        return Ok(total);
    }
}
