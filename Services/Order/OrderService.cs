
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Veltrix.DTOs.OrderDTO;
using Veltrix.DTOs.OrderItemDTO;
using Veltrix.Enums;
using Veltrix.Models;
using Veltrix.UnitOfWork;
using OrderItemModel = Veltrix.Models.OrderItem;
using OrderModel = Veltrix.Models.Order;

namespace Veltrix.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // 1. CREATE ORDER
    
        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            ArgumentNullException.ThrowIfNull(dto);
            ValidateShippingDetails(dto);
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Get customer's cart.
                var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new InvalidOperationException("Your cart is empty.");

                var variantsById = new Dictionary<int, Models.ProductVariant>();
                var orderItems = new List<OrderItemModel>();
                decimal subtotal = 0;

                // Prepare order items and calculate subtotal.
                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Quantity < 1 ||
                        cartItem.Quantity > 100)
                    {
                        throw new InvalidOperationException("Invalid product quantity.");
                    }

                    var variant = await _unitOfWork.productVariant.GetByIdAsync(cartItem.ProductVariantId);
                    if (variant == null)
                    {
                        throw new InvalidOperationException("A product in your cart no longer exists.");
                    }

                    if (!variant.IsActive || !variant.Product.IsPublished)
                    {
                        throw new InvalidOperationException($"{variant.Product.Name} is no longer available.");
                    }

                    if (variant.Inventory == null)
                    {
                        throw new InvalidOperationException($"Inventory not found for {variant.Product.Name}.");
                    }

                    if (variant.Inventory.StockQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for {variant.Product.Name}.");
                    }

                    variantsById[variant.Id] = variant;

                    decimal lineTotal = variant.Price * cartItem.Quantity;
                    subtotal += lineTotal;
                    orderItems.Add(new OrderItemModel
                    {
                        ProductName = variant.Product.Name,
                        SKU = variant.SKU,
                        VariantName = variant.IsDefault ? "Default Variant" : "Product Variant",
                        VariantOptionsSnapshot = null,
                        UnitPrice = variant.Price,
                        Quantity = cartItem.Quantity,
                        LineTotal = lineTotal,
                        CancelledQuantity = 0,
                        FulfilledQuantity = 0,
                        ProductVariantId = variant.Id
                    });
                }

                // Validate and calculate coupon discount.
                Models.Coupon? coupon = null;
                decimal discountAmount = 0;
                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    coupon = await _unitOfWork.Coupons.GetByCodeAsync(dto.CouponCode.Trim());
                    if (coupon == null)
                    {
                        throw new InvalidOperationException("Invalid coupon code.");
                    }

                    ValidateCoupon(coupon, subtotal);

                    bool hasUsedCoupon = await _unitOfWork.CouponUsages.HasUserUsedCouponAsync(coupon.Id, userId);
                    if (hasUsedCoupon)
                    {
                        throw new InvalidOperationException(
                            "You have already used this coupon.");
                    }
                    discountAmount = CalculateDiscount(coupon, subtotal);
                }

                // Get active shipping settings.
                var shippingSettings = await _unitOfWork.ShippingSettings.GetActiveAsync();
                if (shippingSettings == null)
                {
                    throw new InvalidOperationException("Shipping settings are not configured.");
                }

                decimal shippingFee = shippingSettings.ShippingFee;
                if (shippingFee < 0)
                {
                    throw new InvalidOperationException("Invalid shipping fee configuration.");            
                }

                decimal totalAmount = subtotal - discountAmount + shippingFee;


                if (totalAmount < 0)
                {
                    throw new InvalidOperationException("Invalid order total.");
                }

                // Create order.
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

                // Attach order items.
                foreach (var orderItem in orderItems)
                {
                    order.OrderItems.Add(orderItem);
                }

                await _unitOfWork.Orders.AddAsync(order);
                foreach (var cartItem in cart.CartItems)
                {
                    var variant =
                        variantsById[cartItem.ProductVariantId];

                    var inventory = variant.Inventory;

                    if (inventory == null)
                    {
                        throw new InvalidOperationException("Inventory could not be loaded.");

                    }

                    int? quantityAfter =
                        await _unitOfWork.Inventory.TryDecreaseStockAsync(inventory.Id, cartItem.Quantity);
                    if (!quantityAfter.HasValue)
                    {
                        throw new InvalidOperationException($"Insufficient stock for {variant.Product.Name}.");

                    }
                    int quantityBefore = quantityAfter.Value + cartItem.Quantity;
                    var inventoryTransaction = new InventoryTransaction

                    {
                        TransactionType = InventoryTransactionType.OrderPlaced,
                        QuantityChange = -cartItem.Quantity,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = quantityAfter.Value,
                        Reason = "Inventory deducted for order placement",
                        CreatedAt = nowUtc,
                        InventoryId = inventory.Id,
                        Order = order
                    };
                    await _unitOfWork.InventoryTransactions.AddAsync(inventoryTransaction);
                }

                // Atomically increment coupon usage.
                if (coupon != null)
                {
                    bool incremented = await _unitOfWork.Coupons.TryIncrementUsageCountAsync(coupon.Id);
                    if (!incremented)
                    {
                        throw new InvalidOperationException("This coupon has reached its usage limit.");
                    }

                    var couponUsage = new CouponUsage
                    {
                        CouponId = coupon.Id,
                        UsedByUserId = userId,
                        DiscountAmount = discountAmount,
                        UsedAt = nowUtc,
                        Order = order
                    };
                    await _unitOfWork.CouponUsages.AddAsync(couponUsage);
                }

                // Record initial order status.
                var statusHistory = new Models.OrderStatusHistory
                {
                    PreviousStatus = null,
                    NewStatus = OrderStatus.Pending,
                    Note = "Order created.",
                    ChangedAt = nowUtc,
                    Order = order
                };
                await _unitOfWork.OrdersStatusHistory
                    .AddAsync(statusHistory);

                // Clear customer's cart.
                foreach (var cartItem in cart.CartItems.ToList())
                {
                    _unitOfWork.CartItems.Delete(cartItem);
                }

                cart.UpdatedAt = nowUtc;

                _unitOfWork.Carts.Update(cart);

                // Save all changes.
                await _unitOfWork.SaveChangesAsync();

                var result = MapToDto(order, variantsById);

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

     
        // 2. CANCEL CUSTOMER ORDER
        public async Task<OrderDto?> CancelOrderAsync(
            string userId,
            int orderId,
            CancelOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            ArgumentNullException.ThrowIfNull(dto);

            string? cancelReason = dto.CancelReason?.Trim();

            if (string.IsNullOrWhiteSpace(cancelReason) ||
                cancelReason.Length < 3 ||
                cancelReason.Length > 1000)
            {
                throw new InvalidOperationException(
                    "Cancellation reason must be between 3 and 1000 characters.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order =
                    await _unitOfWork.Orders.GetByIdAsync(orderId);

                if (order == null || order.UserId != userId)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                if (order.ShippedAt.HasValue)
                {
                    throw new InvalidOperationException(
                        "This order has already been shipped and cannot be cancelled.");
                }

                var allowedStatuses = new[]
                {
                    OrderStatus.Pending,
                    OrderStatus.Confirmed,
                    OrderStatus.Processing,
                    OrderStatus.OnHold,
                    OrderStatus.PartiallyCancelled
                };

                if (!allowedStatuses.Contains(order.OrderStatus))
                {
                    throw new InvalidOperationException(
                        "This order cannot be cancelled in its current status.");
                }

                if (order.OrderItems == null ||
                    !order.OrderItems.Any())
                {
                    throw new InvalidOperationException(
                        "Order items were not found.");
                }

                // Validate all items before restoring stock.
                foreach (var item in order.OrderItems)
                {
                    if (item.Quantity <= 0 ||
                        item.CancelledQuantity < 0 ||
                        item.CancelledQuantity > item.Quantity ||
                        item.FulfilledQuantity < 0 ||
                        item.FulfilledQuantity > item.Quantity)
                    {
                        throw new InvalidOperationException(
                            "Invalid order item quantities.");
                    }

                    if (item.FulfilledQuantity > 0)
                    {
                        throw new InvalidOperationException(
                            "This order cannot be cancelled because items have already been fulfilled.");
                    }
                }

                var nowUtc = DateTime.UtcNow;

                var previousStatus = order.OrderStatus;

                // Restore inventory.
                foreach (var item in order.OrderItems)
                {
                    int quantityToRestore =
                        item.Quantity - item.CancelledQuantity;

                    if (quantityToRestore <= 0)
                        continue;

                    var inventory = await _unitOfWork.Inventory.GetByProductVariantIdAsync(item.ProductVariantId);


                    if (inventory == null)
                    {
                        throw new InvalidOperationException(
                            $"Inventory not found for variant {item.ProductVariantId}.");
                    }

                    var stockResult =
                        await _unitOfWork.Inventory.TryIncreaseStockAsync(
                            inventory.Id,
                            quantityToRestore);

                    if (!stockResult.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Unable to restore inventory. Please try again.");
                    }

                    var inventoryTransaction =
                        new InventoryTransaction
                        {
                            TransactionType =
                                InventoryTransactionType.OrderCancelled,

                            QuantityChange = quantityToRestore,

                            QuantityBefore =
                                stockResult.Value.QuantityBefore,

                            QuantityAfter =
                                stockResult.Value.QuantityAfter,

                            Reason =
                                $"Order {order.OrderNumber} cancelled: {cancelReason}",

                            CreatedAt = nowUtc,

                            InventoryId = inventory.Id,
                            OrderId = order.Id,

                            PerformedByUserId = userId
                        };

                    await _unitOfWork.InventoryTransactions
                        .AddAsync(inventoryTransaction);

                    item.CancelledQuantity += quantityToRestore;
                }

                // Update order.
                order.OrderStatus = OrderStatus.Cancelled;
                order.CancelReason = cancelReason;
                order.CancelledAt = nowUtc;
                order.UpdatedAt = nowUtc;

                // Record status history.
                var history = new Models.OrderStatusHistory
                {
                    PreviousStatus = previousStatus,
                    NewStatus = OrderStatus.Cancelled,
                    Note = cancelReason,
                    ChangedAt = nowUtc,
                    OrderId = order.Id,
                    ChangedByUserId = userId
                };

                await _unitOfWork.OrdersStatusHistory
                    .AddAsync(history);

                await _unitOfWork.SaveChangesAsync();

                var variantsById =
                    BuildVariantsDictionary(order.OrderItems);

                var result = MapToDto(order, variantsById);

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 3. UPDATE ORDER STATUS (ADMIN)
        public async Task<OrderDto?> UpdateOrderStatusAsync(
            int orderId,
            UpdateOrderStatusDto dto,
            string? changedByUserId = null)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            ArgumentNullException.ThrowIfNull(dto);

            if (!Enum.IsDefined(typeof(OrderStatus), dto.OrderStatus))
            {
                throw new ArgumentException(
                    "Invalid order status.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order =
                    await _unitOfWork.Orders.GetByIdAsync(orderId);

                if (order == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var variantsById =
                    BuildVariantsDictionary(order.OrderItems);

                var previousStatus = order.OrderStatus;
                var newStatus = dto.OrderStatus;

                if (previousStatus == newStatus)
                {
                    var unchangedResult =
                        MapToDto(order, variantsById);

                    await transaction.CommitAsync();

                    return unchangedResult;
                }

                if (!IsValidStatusTransition(
                    previousStatus,
                    newStatus))
                {
                    throw new InvalidOperationException(
                        $"Cannot change order status from {previousStatus} to {newStatus}.");
                }

                var nowUtc = DateTime.UtcNow;

                order.OrderStatus = newStatus;
                order.UpdatedAt = nowUtc;

                if (newStatus == OrderStatus.Confirmed)
                {
                    order.ConfirmedAt ??= nowUtc;
                }

                if (newStatus == OrderStatus.Completed)
                {
                    order.CompletedAt = nowUtc;
                }

                var history = new Models.OrderStatusHistory
                {
                    OrderId = order.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = newStatus,
                    ChangedAt = nowUtc,
                    ChangedByUserId = changedByUserId
                };

                await _unitOfWork.OrdersStatusHistory
                    .AddAsync(history);

                await _unitOfWork.SaveChangesAsync();

                var result = MapToDto(order, variantsById);

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

 
        // 4. COUPON VALIDATION     
        private static void ValidateCoupon(
            Models.Coupon coupon,
            decimal subtotal)
        {
            var nowUtc = DateTime.UtcNow;

            if (!coupon.IsActive)
            {
                throw new InvalidOperationException(
                    "This coupon is inactive.");
            }

            if (nowUtc < coupon.StartsAt ||
                nowUtc >= coupon.ExpiresAt)
            {
                throw new InvalidOperationException(
                    "This coupon is not currently available.");
            }

            if (coupon.StartsAt >= coupon.ExpiresAt)
            {
                throw new InvalidOperationException(
                    "Invalid coupon validity period.");
            }

            if (coupon.UsageLimit.HasValue &&
                coupon.UsageLimit.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Invalid coupon usage limit.");
            }

            if (coupon.UsageCount < 0)
            {
                throw new InvalidOperationException(
                    "Invalid coupon usage count.");
            }

            if (coupon.UsageLimit.HasValue &&
                coupon.UsageCount >= coupon.UsageLimit.Value)
            {
                throw new InvalidOperationException(
                    "This coupon has reached its usage limit.");
            }

            if (coupon.MinimumOrderAmount.HasValue &&
                coupon.MinimumOrderAmount.Value < 0)
            {
                throw new InvalidOperationException(
                    "Invalid coupon minimum order amount.");
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

            if (coupon.MaximumDiscountAmount.HasValue &&
                coupon.MaximumDiscountAmount.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Invalid maximum discount configuration.");
            }

            if (coupon.DiscountType == DiscountType.Percentage &&
                coupon.DiscountValue > 100)
            {
                throw new InvalidOperationException(
                    "Percentage discount cannot exceed 100%.");
            }

            if (coupon.DiscountType != DiscountType.Percentage &&
                coupon.DiscountType != DiscountType.FixedAmount)
            {
                throw new InvalidOperationException(
                    "Unsupported coupon discount type.");
            }
        }

        // 5. CALCULATE COUPON DISCOUNT
        private static decimal CalculateDiscount(
            Models.Coupon coupon,
            decimal subtotal)
        {
            decimal discountAmount;

            if (coupon.DiscountType == DiscountType.Percentage)
            {
                discountAmount =
                    subtotal * coupon.DiscountValue / 100m;
            }
            else
            {
                discountAmount = coupon.DiscountValue;
            }

            if (coupon.MaximumDiscountAmount.HasValue)
            {
                discountAmount = Math.Min(
                    discountAmount,
                    coupon.MaximumDiscountAmount.Value);
            }

            // Discount cannot exceed subtotal.
            discountAmount = Math.Min(
                discountAmount,
                subtotal);

            return Math.Round(
                discountAmount,
                2,
                MidpointRounding.AwayFromZero);
        }

        // 6. VALID STATUS TRANSITIONS

        private static bool IsValidStatusTransition(
            OrderStatus currentStatus,
            OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending =>
                    newStatus == OrderStatus.Confirmed,

                OrderStatus.Confirmed =>
                    newStatus == OrderStatus.Processing ||
                    newStatus == OrderStatus.OnHold,

                OrderStatus.Processing =>
                    newStatus == OrderStatus.OnHold ||
                    newStatus == OrderStatus.Completed,

                OrderStatus.OnHold =>
                    newStatus == OrderStatus.Confirmed ||
                    newStatus == OrderStatus.Processing,

                _ => false
            };
        }

        // 7. BUILD PRODUCT VARIANTS DICTIONARY

        private static Dictionary<int, Models.ProductVariant>
            BuildVariantsDictionary(
                IEnumerable<OrderItemModel> orderItems)
        {
            var result =
                new Dictionary<int, Models.ProductVariant>();

            foreach (var item in orderItems)
            {
                if (item.ProductVariant == null)
                {
                    throw new InvalidOperationException(
                        "Product variant information was not loaded for this order.");
                }

                result.TryAdd(
                    item.ProductVariantId,
                    item.ProductVariant);
            }

            return result;
        }

        // 8. VALIDATE SHIPPING DETAILS
    
        private static void ValidateShippingDetails(
            CreateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ShippingRecipientName) ||
                dto.ShippingRecipientName.Trim().Length < 2 ||
                dto.ShippingRecipientName.Trim().Length > 150)
            {
                throw new InvalidOperationException(
                    "Invalid shipping recipient name.");
            }

            if (string.IsNullOrWhiteSpace(dto.ShippingPhoneNumber) ||
                dto.ShippingPhoneNumber.Trim().Length < 7 ||
                dto.ShippingPhoneNumber.Trim().Length > 30)
            {
                throw new InvalidOperationException(
                    "Invalid shipping phone number.");
            }

            if (string.IsNullOrWhiteSpace(dto.ShippingAddressLine1) ||
                dto.ShippingAddressLine1.Trim().Length < 5 ||
                dto.ShippingAddressLine1.Trim().Length > 250)
            {
                throw new InvalidOperationException(
                    "Invalid shipping address.");
            }

            if (string.IsNullOrWhiteSpace(dto.ShippingCity) ||
                dto.ShippingCity.Trim().Length < 2 ||
                dto.ShippingCity.Trim().Length > 100)
            {
                throw new InvalidOperationException(
                    "Invalid shipping city.");
            }

            if (string.IsNullOrWhiteSpace(dto.ShippingGovernorate) ||
                dto.ShippingGovernorate.Trim().Length < 2 ||
                dto.ShippingGovernorate.Trim().Length > 100)
            {
                throw new InvalidOperationException(
                    "Invalid shipping governorate.");
            }

            if (dto.ShippingAddressLine2?.Trim().Length > 250 ||
                dto.ShippingPostalCode?.Trim().Length > 20 ||
                dto.ShippingInstructions?.Trim().Length > 1000 ||
                dto.CouponCode?.Trim().Length > 50)
            {
                throw new InvalidOperationException(
                    "One or more shipping fields exceed the allowed length.");
            }
        }

        // 9. GENERATE ORDER NUMBER
    
        private static string GenerateOrderNumber()
        {
            return $"VX-{DateTime.UtcNow:yyyyMMdd}-" +
                   $"{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }

      
        // 10. MAP ORDER ENTITY TO DTO
        private static OrderDto MapToDto(
            OrderModel order,
            Dictionary<int, Models.ProductVariant> variantsById)
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

                ShippingRecipientName =
                    order.ShippingRecipientName,

                ShippingPhoneNumber =
                    order.ShippingPhoneNumber,

                ShippingAddressLine1 =
                    order.ShippingAddressLine1,

                ShippingAddressLine2 =
                    order.ShippingAddressLine2,

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
                    if (!variantsById.TryGetValue(
                        item.ProductVariantId,
                        out var variant))
                    {
                        throw new InvalidOperationException(
                            $"Product variant {item.ProductVariantId} was not loaded.");
                    }

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

                        VariantOptionsSnapshot =
                            item.VariantOptionsSnapshot,

                        CancelledQuantity =
                            item.CancelledQuantity,

                        FulfilledQuantity =
                            item.FulfilledQuantity
                    };
                }).ToList()
            };
        }
    }
}
