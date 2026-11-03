namespace Veltrix.Repositories.VariantOptionValue
{
    public interface IVariantOptionValueRepository
    {
        Task<IReadOnlyList<Models.VariantOptionValue>> GetByIdsAsync(IReadOnlyCollection<int> ids);
        Task<IReadOnlyList<int>> GetOptionGroupIdsByProductIdAsync(int productId);
    }
}
