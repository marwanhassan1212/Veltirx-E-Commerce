
using Veltrix.DTOs.CategoryDTO;
using Veltrix.DTOs.Common;
using Veltrix.DTOs.InventoryDTO;
using Veltrix.DTOs.InventoryTransactionDTO;
using Veltrix.Enums;
using Veltrix.Models;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all inventory records
        public async Task<IReadOnlyList<InventoryDto>> GetAllAsync()
        {
            var inventories = await _unitOfWork.Inventory.GetAllAsync();

            return inventories.Select(MapToInventoryDto).ToList();
        }

        // Get inventory by ID
        public async Task<InventoryDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid inventory ID.");
            var inventory = await _unitOfWork.Inventory.GetByIdAsync(id);
            return inventory == null ? null : MapToInventoryDto(inventory);
        }

        // Get inventory by Product Variant ID
        public async Task<InventoryDto?> GetByProductVariantIdAsync(
            int productVariantId)
        {
            if (productVariantId <= 0)
                throw new ArgumentException("Invalid product variant ID.");

            var inventory = await _unitOfWork.Inventory.GetByProductVariantIdAsync(productVariantId);
            return inventory == null ? null : MapToInventoryDto(inventory);
        }

        // Get transactions by inventory ID
        public async Task<IReadOnlyList<InventoryTransactionDto>> GetTransactionsAsync(int inventoryId)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("Invalid inventory ID.");

            var inventory = await _unitOfWork.Inventory.GetByIdAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Inventory not found.");

            var transactions = await _unitOfWork.InventoryTransactions.GetByInventoryIdAsync(inventoryId);
            return transactions.Select(x => MapToTransactionDto(x, inventory)).ToList();
        }

        // Get transactions by order ID
        public async Task<IReadOnlyList<InventoryTransactionDto>> GetTransactionsByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            var transactions = await _unitOfWork.InventoryTransactions.GetByOrderIdAsync(orderId);
            var result = new List<InventoryTransactionDto>();

            foreach (var transaction in transactions)
            {
                var inventory = await _unitOfWork.Inventory.GetByIdAsync(transaction.InventoryId);
                if (inventory == null)
                    continue;
                result.Add(MapToTransactionDto(transaction, inventory));
            }

            return result;
        }

        // Update stock and record transaction
        public async Task<InventoryDto> UpdateStockAsync(int inventoryId, UpdateInventoryDto dto)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("Invalid inventory ID.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.StockQuantity < 0)
                throw new ArgumentOutOfRangeException(nameof(dto.StockQuantity), "Stock quantity cannot be negative.");

            if (string.IsNullOrWhiteSpace(dto.Reason) || dto.Reason.Trim().Length < 3 || dto.Reason.Trim().Length > 500)
            {
                throw new ArgumentException("A valid reason between 3 and 500 characters is required.");
            }

            var inventory = await _unitOfWork.Inventory.GetByIdAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Inventory not found.");

            var currentQuantity = inventory.StockQuantity;
            var targetQuantity = dto.StockQuantity;

            if (currentQuantity == targetQuantity)
                return MapToInventoryDto(inventory);

            var difference = Math.Abs((long)targetQuantity - currentQuantity);
            if (difference > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dto.StockQuantity), "Stock adjustment exceeds the supported quantity.");

            var quantity = (int)difference;
            await using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                int quantityBefore;
                int quantityAfter;
                if (targetQuantity > currentQuantity)
                {
                    var increaseResult = await _unitOfWork.Inventory.TryIncreaseStockAsync(inventoryId, quantity);
                    if (increaseResult == null)
                    {
                        throw new InvalidOperationException("Stock could not be increased. Please try again.");
                    }

                    quantityBefore = increaseResult.Value.QuantityBefore;
                    quantityAfter = increaseResult.Value.QuantityAfter;
                }
                else
                {
                    var decreaseResult = await _unitOfWork.Inventory.TryDecreaseStockAsync(inventoryId, quantity);
                    if (decreaseResult == null)
                    {
                        throw new InvalidOperationException("Stock could not be decreased. Please try again.");

                    }
                    quantityAfter = decreaseResult.Value;
                    quantityBefore = quantityAfter + quantity;
                }

                var inventoryTransaction = new InventoryTransaction
                {
                    InventoryId = inventoryId,
                    TransactionType = InventoryTransactionType.StockAdjusted,
                    QuantityChange = targetQuantity > currentQuantity ? quantity : -quantity,
                    QuantityBefore = quantityBefore,
                    QuantityAfter = quantityAfter,
                    Reason = dto.Reason.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.InventoryTransactions.AddAsync(inventoryTransaction);
                await _unitOfWork.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                var updatedInventory = await _unitOfWork.Inventory.GetByIdAsync(inventoryId);
                if (updatedInventory == null)
                    throw new InvalidOperationException("Updated inventory could not be retrieved.");
                return MapToInventoryDto(updatedInventory);
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        // Inventory mapping
        private static InventoryDto MapToInventoryDto(Models.Inventory inventory)
        {
            var variant = inventory.ProductVariant;

            return new InventoryDto
            {
                Id = inventory.Id,
                ProductId = variant.ProductId,
                ProductVariantId = variant.Id,
                ProductName = variant.Product.Name,
                SKU = variant.SKU,

                VariantName = string.Join(" / ",
                    variant.VariantOptions
                        .Select(x =>
                            $"{x.VariantOptionValue.VariantOption.Name}: {x.VariantOptionValue.Value}")),

                StockQuantity = inventory.StockQuantity,
                UpdatedAt = inventory.UpdatedAt
            };
        }

        // Transaction mapping
        private static InventoryTransactionDto MapToTransactionDto(InventoryTransaction transaction, Models.Inventory inventory)
        {
            return new InventoryTransactionDto
            {
                Id = transaction.Id,
                TransactionType = transaction.TransactionType,
                QuantityChange = transaction.QuantityChange,
                QuantityBefore = transaction.QuantityBefore,
                QuantityAfter = transaction.QuantityAfter,
                Reason = transaction.Reason,
                CreatedAt = transaction.CreatedAt,
                InventoryId = transaction.InventoryId,
                ProductId = inventory.ProductVariant.ProductId,
                ProductName = inventory.ProductVariant.Product.Name,
                PerformedByUserId = transaction.PerformedByUserId,
                PerformedByUserName = null,
                OrderId = transaction.OrderId
            };
        }
        public async Task<PagedResultDto<InventoryDto>> GetFilteredAsync(InventoryFilterDto filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            filter.PageNumber = Math.Max(1, filter.PageNumber);

            if (filter.PageSize != 10 && filter.PageSize != 25 && filter.PageSize != 50 && filter.PageSize != 100)
            {
                filter.PageSize = 10;
            }

            if (filter.MinQuantity.HasValue && filter.MaxQuantity.HasValue && filter.MinQuantity.Value > filter.MaxQuantity.Value)
            {
                throw new ArgumentException("Minimum quantity cannot exceed maximum quantity.");
            }

            var allowedStatuses = new[]
            {
            "All", "InStock", "LowStock", "OutOfStock"
            };

            if (!string.IsNullOrWhiteSpace(filter.StockStatus) && !allowedStatuses.Contains(filter.StockStatus))
            {
                throw new ArgumentException("Invalid stock status.");
            }

            var allowedSortOptions = new[]
            {
                "RecentlyUpdated", "ProductName", "SKU", "Quantity"
            };

            if (!allowedSortOptions.Contains(filter.SortBy))
                filter.SortBy = "RecentlyUpdated";

            filter.SearchTerm = filter.SearchTerm?.Trim();

            var (inventories, totalCount) = await _unitOfWork.Inventory.GetFilteredAsync(filter);
            return new PagedResultDto<InventoryDto>
            {
                Items = inventories.Select(MapToInventoryDto).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
    }
}
