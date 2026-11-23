using Microsoft.EntityFrameworkCore;
using TP6_WEBAPI.Data;
using TP6_WEBAPI.Models;

namespace TP6_WEBAPI.Repositories;

public class CompteRepository : ICompteRepository
{
    private readonly AppDbContext _context;

    public CompteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Compte>> GetAll()
    {
        return await _context.Comptes
            .Include(c => c.Client)
            .OrderBy(c => c.DateCreation)
            .ToListAsync();
    } 

    public async Task<Compte?> GetById(int compteId)
    {
        return await _context.Comptes
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.CompteId == compteId);
    }

    public async Task<Compte> Add(Compte compte)
    {
        await _context.Comptes.AddAsync(compte);
        await _context.SaveChangesAsync();

        return compte;
    }

    public async Task<int> NombreOperations(int numeroCompte)
    {
        return await _context.Operations
            .CountAsync(o => o.Compte!.NumeroCompte == numeroCompte);
    }

    public async Task<Compte?> Update(Compte compte)
    {
        var compteExistant = await _context.Comptes
            .FirstOrDefaultAsync(c => c.CompteId == compte.CompteId);

        if (compteExistant is null)
        {
            return null;
        }

        compteExistant.NumeroCompte = compte.NumeroCompte;
        compteExistant.ClientId = compte.ClientId;

        await _context.SaveChangesAsync();

        return compteExistant;
    }

    public async Task<Compte?> Delete(int compteId)
    {
        var compte = await _context.Comptes
            .FirstOrDefaultAsync(c => c.CompteId == compteId);

        if (compte is null)
        {
            return null;
        }

        var contientOperations = await _context.Operations
            .AnyAsync(o => o.CompteId == compteId);

        if (contientOperations)
        {
            throw new InvalidOperationException(
                "Impossible de supprimer un compte contenant des opérations.");
        }

        _context.Comptes.Remove(compte);
        await _context.SaveChangesAsync();

        return compte;
    }

    public async Task<IEnumerable<Compte>> Search(int clientId)
    {
        return await _context.Comptes
            .Include(c => c.Client)
            .Where(c => c.ClientId == clientId)
            .OrderBy(c => c.DateCreation)
            .ToListAsync();
    }

    public async Task<Operation> Add(Operation operation)
    {
        await _context.Operations.AddAsync(operation);
        await _context.SaveChangesAsync();

        return operation;
    }

    public async Task<Compte?> MajSolde(int numeroCompte, Operation operation)
    {
        var compte = await _context.Comptes
            .FirstOrDefaultAsync(c => c.NumeroCompte == numeroCompte);

        if (compte is null)
        {
            return null;
        }

        if (operation.Montant <= 0)
        {
            throw new InvalidOperationException(
                "Le montant de l'opération doit être supérieur à zéro.");
        }

        var typeOperation = operation.TypeOperation.Trim();

        if (string.Equals(typeOperation, "Retrait", StringComparison.OrdinalIgnoreCase))
        {
            if (compte.Solde < operation.Montant)
            {
                throw new InvalidOperationException("Solde insuffisant.");
            }

            compte.Solde -= operation.Montant;
        }
        else if (string.Equals(typeOperation, "Versement", StringComparison.OrdinalIgnoreCase))
        {
            compte.Solde += operation.Montant;
        }
        else
        {
            throw new InvalidOperationException(
                "Cette méthode accepte uniquement les opérations Retrait ou Versement.");
        }

        await _context.SaveChangesAsync();

        return compte;
    }

    public async Task<IEnumerable<Operation>> ExtraitCompte(
        int numeroCompte,
        DateTime date)
    {
        return await _context.Operations
            .Include(o => o.Compte)
            .Where(o => o.Compte!.NumeroCompte == numeroCompte && o.DateOperation >= date)
            .OrderBy(o => o.DateOperation)
            .ToListAsync();
    }

    public async Task<double> SoldeTotal(int clientId)
    {
        return await _context.Comptes
            .Where(c => c.ClientId == clientId)
            .SumAsync(c => c.Solde);
    }

    public async Task MajVirement(
        int numeroCompte1,
        int numeroCompte2,
        double montant)
    {
        if (numeroCompte1 == numeroCompte2)
        {
            throw new InvalidOperationException(
                "Les deux comptes du virement doivent être différents.");
        }

        if (montant <= 0)
        {
            throw new InvalidOperationException(
                "Le montant du virement doit être supérieur à zéro.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var compteSource = await _context.Comptes
            .FirstOrDefaultAsync(c => c.NumeroCompte == numeroCompte1);

        var compteDestination = await _context.Comptes
            .FirstOrDefaultAsync(c => c.NumeroCompte == numeroCompte2);

        if (compteSource is null || compteDestination is null)
        {
            throw new InvalidOperationException(
                "Le compte source ou le compte destination est introuvable.");
        }

        if (compteSource.Solde < montant)
        {
            throw new InvalidOperationException("Solde insuffisant.");
        }

        compteSource.Solde -= montant;
        compteDestination.Solde += montant;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<double> TotalRetraits(int clientId)
    {
        return await _context.Operations
            .Where(o => o.Compte!.ClientId == clientId && o.TypeOperation == "Retrait")
            .SumAsync(o => o.Montant);
    }
}
