namespace Veltrix.Repositories.VariantOptions
{
    public interface IVariantOptionsRepository
    {
        Task<Models.VariantOptions?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.VariantOptions>> GetByProductIdAsync(int productId);
        Task AddAsync(Models.VariantOptions option);
        void Update(Models.VariantOptions option);

        void Delete(Models.VariantOptions option);
    }
}
