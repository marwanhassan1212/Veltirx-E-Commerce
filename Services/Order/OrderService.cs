
using Veltrix.DTOs.OrderDTO;
using Veltrix.DTOs.OrderItemDTO;
using Veltrix.Enums;
using Veltrix.Models;
using Veltrix.UnitOfWork;

using OrderModel = Veltrix.Models.Order;
using OrderItemModel = Veltrix.Models.OrderItem;

namespace Veltrix.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Get customer's cart
                var cart = await _unitOfWork.Carts
                    .GetByUserIdAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new InvalidOperationException("Your cart is empty.");

                // Store loaded variants to avoid querying them again.
                var variantsById = new Dictionary<int, Models.ProductVariant>();

                // 2. Prepare order items and calculate subtotal
                var orderItems = new List<OrderItemModel>();
                decimal subtotal = 0;

                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Quantity < 1 ||
                        cartItem.Quantity > 100)
                    {
                        throw new InvalidOperationException(
                            "Invalid product quantity.");
                    }

                    var variant = await _unitOfWork.productVariant
                        .GetByIdAsync(cartItem.ProductVariantId);

                    if (variant == null)
                    {
                        throw new InvalidOperationException(
                            "A product in your cart no longer exists.");
                    }

                    if (!variant.IsActive || !variant.Product.IsPublished)
                    {
                        throw new InvalidOperationException(
                            $"{variant.Product.Name} is no longer available.");
                    }

                    if (variant.Inventory == null)
                    {
                        throw new InvalidOperationException(
                            $"Inventory not found for {variant.Product.Name}.");
                    }

                    if (variant.Inventory.StockQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for {variant.Product.Name}.");
                    }

                    // Keep the loaded variant for inventory deduction and DTO mapping.
                    variantsById[variant.Id] = variant;

                    decimal lineTotal =
                        variant.Price * cartItem.Quantity;

                    subtotal += lineTotal;

                    orderItems.Add(new OrderItemModel
                    {
                        ProductName = variant.Product.Name,
                        SKU = variant.SKU,

                        // Variant options snapshot will be implemented
                        // after reviewing the variant-option models.
                        VariantName = variant.IsDefault
                            ? "Default Variant"
                            : "Product Variant",

                        VariantOptionsSnapshot = null,

                        UnitPrice = variant.Price,
                        Quantity = cartItem.Quantity,
                        LineTotal = lineTotal,

                        CancelledQuantity = 0,
                        FulfilledQuantity = 0,

                        ProductVariantId = variant.Id
                    });
                }

                // 3. Validate and calculate coupon discount
                Coupon? coupon = null;

                decimal discountAmount = 0;

                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    coupon = await _unitOfWork.Coupons
                        .GetByCodeAsync(dto.CouponCode.Trim());

                    if (coupon == null)
                    {
                        throw new InvalidOperationException(
                            "Invalid coupon code.");
                    }

                    var now = DateTime.UtcNow;

                    if (!coupon.IsActive ||
                        now < coupon.StartsAt ||
                        now > coupon.ExpiresAt)
                    {
                        throw new InvalidOperationException(
                            "This coupon is not currently available.");
                    }

                    if (coupon.UsageLimit.HasValue &&
                        coupon.UsageCount >= coupon.UsageLimit.Value)
                    {
                        throw new InvalidOperationException(
                            "This coupon has reached its usage limit.");
                    }

                    if (coupon.MinimumOrderAmount.HasValue &&
                        subtotal < coupon.MinimumOrderAmount.Value)
                    {
                        throw new InvalidOperationException(
                            "The order does not meet the coupon minimum amount.");
                    }

                    if (coupon.DiscountValue <= 0)
                    {
                        throw new InvalidOperationException(
                            "Invalid coupon discount configuration.");
                    }

                    if (coupon.DiscountType == DiscountType.Percentage)
                    {
                        discountAmount =
                            subtotal * coupon.DiscountValue / 100m;
                    }
                    else if (coupon.DiscountType == DiscountType.FixedAmount)
                    {
                        discountAmount = coupon.DiscountValue;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Unsupported coupon discount type.");
                    }

                    if (coupon.MaximumDiscountAmount.HasValue)
                    {
                        discountAmount = Math.Min(
                            discountAmount,
                            coupon.MaximumDiscountAmount.Value);
                    }

                    // Discount cannot exceed the order subtotal.
                    discountAmount = Math.Min(
                        discountAmount,
                        subtotal);
                }

                // 4. Get active shipping settings
                var shippingSettings = await _unitOfWork.ShippingSettings
                    .GetActiveAsync();

                if (shippingSettings == null)
                {
                    throw new InvalidOperationException(
                        "Shipping settings are not configured.");
                }

                decimal shippingFee = shippingSettings.ShippingFee;

                if (shippingFee < 0)
                {
                    throw new InvalidOperationException(
                        "Invalid shipping fee configuration.");
                }

                decimal totalAmount =
                    subtotal - discountAmount + shippingFee;

                // 5. Create the order
                var nowUtc = DateTime.UtcNow;
                var order = new OrderModel
                {
                    OrderNumber = GenerateOrderNumber(),
                    OrderStatus = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.CashOnDelivery,
                    PaymentStatus = PaymentStatus.Pending,
                    Subtotal = subtotal,
                    DiscountAmount = discountAmount,
                    ShippingFee = shippingFee,
                    TotalAmount = totalAmount,
                    ShippingRecipientName = dto.ShippingRecipientName.Trim(),
                    ShippingPhoneNumber = dto.ShippingPhoneNumber.Trim(),
                    ShippingAddressLine1 = dto.ShippingAddressLine1.Trim(),
                    ShippingAddressLine2 = dto.ShippingAddressLine2?.Trim(),
                    ShippingCity = dto.ShippingCity.Trim(),
                    ShippingGovernorate = dto.ShippingGovernorate.Trim(),
                    ShippingPostalCode = dto.ShippingPostalCode?.Trim(),
                    ShippingInstructions = dto.ShippingInstructions?.Trim(),
                    UserId = userId,
                    CouponId = coupon?.Id,
                    CreatedAt = nowUtc
                };

                // 6. Attach order items to the order
                foreach (var orderItem in orderItems)
                {
                    order.OrderItems.Add(orderItem);
                }

                await _unitOfWork.Orders.AddAsync(order);

                // 7. Deduct inventory and record inventory transactions
                foreach (var cartItem in cart.CartItems)
                {
                    var variant =
                        variantsById[cartItem.ProductVariantId];

                    var inventory = variant.Inventory;

                    if (inventory == null)
                    {
                        throw new InvalidOperationException(
                            "Inventory could not be loaded.");
                    }

                    int quantityBefore = inventory.StockQuantity;

                    int quantityAfter =
                        quantityBefore - cartItem.Quantity;

                    if (quantityAfter < 0)
                    {
                        throw new InvalidOperationException(
                            "Insufficient stock available.");
                    }

                    inventory.StockQuantity = quantityAfter;
                    inventory.UpdatedAt = nowUtc;

                    _unitOfWork.Inventory.Update(inventory);

                    var inventoryTransaction =
                        new InventoryTransaction
                        {
                            TransactionType =
                                InventoryTransactionType.OrderPlaced,

                            QuantityChange = -cartItem.Quantity,
                            QuantityBefore = quantityBefore,
                            QuantityAfter = quantityAfter,

                            Reason =
                                "Inventory deducted for order placement",

                            CreatedAt = nowUtc,

                            InventoryId = inventory.Id,
                            Order = order
                        };

                    await _unitOfWork.InventoryTransactions
                        .AddAsync(inventoryTransaction);
                }

                // 8. Record coupon usage
                if (coupon != null)
                {
                    coupon.UsageCount++;
                    coupon.UpdatedAt = nowUtc;

                    _unitOfWork.Coupons.Update(coupon);

                    var couponUsage = new CouponUsage
                    {
                        CouponId = coupon.Id,
                        UsedByUserId = userId,
                        DiscountAmount = discountAmount,
                        UsedAt = nowUtc,
                        Order = order
                    };

                    await _unitOfWork.CouponUsages
                        .AddAsync(couponUsage);
                }

                // 9. Record the initial order status
                var statusHistory = new Models.OrderStatusHistory
                {
                    PreviousStatus = null,
                    NewStatus = OrderStatus.Pending,
                    Note = "Order created.",
                    ChangedAt = nowUtc,
                    Order = order
                };

                await _unitOfWork.OrdersStatusHistory.AddAsync(statusHistory);


                // 10. Clear the customer's cart
                foreach (var cartItem in cart.CartItems.ToList())
                {
                    _unitOfWork.CartItems.Delete(cartItem);
                }

                cart.UpdatedAt = nowUtc;
                _unitOfWork.Carts.Update(cart);

                // 11. Save all changes and commit transaction
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                // 12. Return the created order
                return MapToDto(order, variantsById);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Generate a unique-looking order number.
        private static string GenerateOrderNumber()
        {
            return $"VX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }

        // Convert the created order entity into an OrderDto.
        private static OrderDto MapToDto(OrderModel order, Dictionary<int, Models.ProductVariant> variantsById)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                CouponId = order.CouponId,
                ShippingRecipientName = order.ShippingRecipientName,
                ShippingPhoneNumber = order.ShippingPhoneNumber,
                ShippingAddressLine1 = order.ShippingAddressLine1,
                ShippingAddressLine2 = order.ShippingAddressLine2,
                ShippingCity = order.ShippingCity,
                ShippingGovernorate = order.ShippingGovernorate,
                ShippingPostalCode = order.ShippingPostalCode,
                ShippingInstructions = order.ShippingInstructions,
                CancelReason = order.CancelReason,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ConfirmedAt = order.ConfirmedAt,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                CancelledAt = order.CancelledAt,

                Items = order.OrderItems.Select(item =>
                {
                    var variant = variantsById[item.ProductVariantId];


                    return new OrderItemDto
                    {
                        Id = item.Id,
                        ProductId = variant.ProductId,
                        ProductName = item.ProductName,
                        SKU = item.SKU,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        LineTotal = item.LineTotal,
                        ProductVariantId = item.ProductVariantId,
                        VariantName = item.VariantName,
                        VariantOptionsSnapshot = item.VariantOptionsSnapshot,
                        CancelledQuantity = item.CancelledQuantity,
                        FulfilledQuantity = item.FulfilledQuantity

                    };
                }).ToList()
            };
        }
    }
}
