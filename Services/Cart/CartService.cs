using Veltrix.DTOs.CartDTO;
using Veltrix.DTOs.CartItemDTO;
using Veltrix.Models;
using Veltrix.UnitOfWork;
using CartModel = Veltrix.Models.Cart;

namespace Veltrix.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get Customer Cart
        public async Task<CartDTO> GetCartAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (cart == null)
                return new CartDTO();

            return MapToDto(cart);
        }

        // Add Item To Cart
        public async Task<CartDTO> AddItemAsync(string userId, AddCartItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Quantity < 1 || dto.Quantity > 100)
                throw new ArgumentException("Quantity must be between 1 and 100.");

            var variant = await _unitOfWork.productVariant
                .GetByIdAsync(dto.ProductVariantId);

            if (variant == null)
                throw new InvalidOperationException("Product variant not found.");

            if (!variant.IsActive || !variant.Product.IsPublished)
                throw new InvalidOperationException("Product is not available.");

            if (variant.Inventory == null)
                throw new InvalidOperationException("Inventory not found.");

            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new CartModel
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            var existingItem = await _unitOfWork.CartItems
                .GetByCartAndProductAsync(cart.Id, dto.ProductVariantId);

            int newQuantity = dto.Quantity;

            if (existingItem != null)
                newQuantity += existingItem.Quantity;

            if (newQuantity > 100)
                throw new InvalidOperationException("Maximum quantity is 100.");

            if (variant.Inventory.StockQuantity < newQuantity)
                throw new InvalidOperationException("Not enough stock available.");

            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
                existingItem.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    AddedAt = DateTime.UtcNow
                };

                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Carts.Update(cart);

            await _unitOfWork.SaveChangesAsync();

            var updatedCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (updatedCart == null)
                throw new InvalidOperationException("Cart could not be loaded.");

            return MapToDto(updatedCart);
        }

        // Update Cart Item Quantity
        public async Task<CartDTO> UpdateItemAsync(
            string userId,
            int cartItemId,
            UpdateCartItemDTO dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (cartItemId <= 0)
                throw new ArgumentException("Invalid cart item ID.");

            if (dto.Quantity < 1 || dto.Quantity > 100)
                throw new ArgumentException("Quantity must be between 1 and 100.");

            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            var cartItem = cart.CartItems
                .FirstOrDefault(x => x.Id == cartItemId);

            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found.");

            var variant = cartItem.ProductVariant;

            if (!variant.IsActive || !variant.Product.IsPublished)
                throw new InvalidOperationException("Product is not available.");

            if (variant.Inventory == null)
                throw new InvalidOperationException("Inventory not found.");

            if (variant.Inventory.StockQuantity < dto.Quantity)
                throw new InvalidOperationException("Not enough stock available.");

            cartItem.Quantity = dto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            cart.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CartItems.Update(cartItem);
            _unitOfWork.Carts.Update(cart);

            await _unitOfWork.SaveChangesAsync();

            var updatedCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (updatedCart == null)
                throw new InvalidOperationException("Cart could not be loaded.");

            return MapToDto(updatedCart);
        }

        // Remove Item From Cart
        public async Task<CartDTO> RemoveItemAsync(
            string userId,
            int cartItemId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (cartItemId <= 0)
                throw new ArgumentException("Invalid cart item ID.");

            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            var cartItem = cart.CartItems
                .FirstOrDefault(x => x.Id == cartItemId);

            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found.");

            _unitOfWork.CartItems.Delete(cartItem);

            cart.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Carts.Update(cart);

            await _unitOfWork.SaveChangesAsync();

            var updatedCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

            if (updatedCart == null)
                throw new InvalidOperationException("Cart could not be loaded.");

            return MapToDto(updatedCart);
        }

        // Map Cart Entity To DTO
        private static CartDTO MapToDto(CartModel cart)
        {
            var result = new CartDTO
            {
                Id = cart.Id
            };

            foreach (var item in cart.CartItems)
            {
                var variant = item.ProductVariant;
                var product = variant.Product;

                var image = product.ProductImages
                    .Where(x => x.IsPrimary)
                    .FirstOrDefault()
                    ?? product.ProductImages
                        .OrderBy(x => x.DisplayOrder)
                        .FirstOrDefault();

                result.Items.Add(new CartItemDTO
                {
                    Id = item.Id,
                    ProductId = product.Id,
                    ProductVariantId = variant.Id,
                    ProductName = product.Name,
                    SKU = variant.SKU,
                    ProductImageUrl = image?.ImageUrl,
                    UnitPrice = variant.Price,
                    Quantity = item.Quantity,
                    TotalPrice = variant.Price * item.Quantity
                });
            }

            result.TotalItems = result.Items.Sum(x => x.Quantity);
            result.SubTotal = result.Items.Sum(x => x.TotalPrice);

            return result;
        }
    }
}