namespace Veltrix.Repositories.VariantOptionValue
{
    public interface IVariantOptionValueRepository
    {
        Task<IReadOnlyList<Models.VariantOptionValue>> GetByIdsAsync(IReadOnlyCollection<int> ids);
        Task<IReadOnlyList<int>> GetOptionGroupIdsByProductIdAsync(int productId);
        Task<IReadOnlyList<Models.VariantOptions>> GetGroupsWithValuesByProductIdAsync(int productId);
        Task<Models.VariantOptionValue?> GetByIdAsync(int id);
        Task AddAsync(Models.VariantOptionValue optionValue);
        void Update(Models.VariantOptionValue optionValue);
        void Delete(Models.VariantOptionValue optionValue);
    }
}
