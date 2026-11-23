using TP6_WEBAPI.Models;

namespace TP6_WEBAPI.Repositories;

public interface ICompteRepository
{
    Task<IEnumerable<Compte>> GetAll();

    Task<Compte?> GetById(int compteId);

    Task<Compte> Add(Compte compte);

    Task<int> NombreOperations(int numeroCompte);

    Task<Compte?> Update(Compte compte);

    Task<Compte?> Delete(int compteId);

    Task<IEnumerable<Compte>> Search(int clientId);

    Task<Operation> Add(Operation operation);

    Task<Compte?> MajSolde(int numeroCompte, Operation operation);

    Task<IEnumerable<Operation>> ExtraitCompte(int numeroCompte, DateTime date);

    Task<double> SoldeTotal(int clientId);

    Task MajVirement(int numeroCompte1, int numeroCompte2, double montant);

    Task<double> TotalRetraits(int clientId);
}
