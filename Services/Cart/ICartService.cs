using Veltrix.DTOs.CartDTO;
using Veltrix.DTOs.CartItemDTO;

namespace Veltrix.Services.Cart
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(string userId);
        Task<CartDTO> AddItemAsync(string userId, AddCartItemDto dto);
        Task<CartDTO> UpdateItemAsync(string userId, int cartItemId, UpdateCartItemDTO dto);
        Task<CartDTO> RemoveItemAsync(string userId, int cartItemId);
    }
}