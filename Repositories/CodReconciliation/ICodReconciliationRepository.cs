namespace Veltrix.Repositories.CodReconciliation
{
    public interface ICodReconciliationRepository
    {
        Task<Models.CodReconciliation?> GetByIdAsync(int id);
        Task<Models.CodReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber);
        Task<IReadOnlyList<Models.CodReconciliation>> GetAllAsync();
        Task AddAsync(Models.CodReconciliation reconciliation);
        void Update(Models.CodReconciliation reconciliation);
    }
}
