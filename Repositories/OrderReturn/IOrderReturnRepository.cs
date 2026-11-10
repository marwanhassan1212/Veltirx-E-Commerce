using Veltrix.Models;

namespace Veltrix.Repositories.OrderReturn
{
    public interface IOrderReturnRepository
    {
        Task<Models.OrderReturn?> GetByIdAsync(int id);
        Task<Models.OrderReturn?> GetByReturnNumberAsync(string returnNumber);
        Task<IReadOnlyList<Models.OrderReturn>> GetByOrderIdAsync(int orderId);
        Task<IReadOnlyList<Models.OrderReturn>> GetAllAsync();
        Task AddAsync(Models.OrderReturn orderReturn);
        void Update(Models.OrderReturn orderReturn);
    }
}