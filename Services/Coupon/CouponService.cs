
using Veltrix.DTOs.CouponDTO;
using Veltrix.Models;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.Coupon
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<CouponListDto>> GetAllAsync()
        {
            var coupons = await _unitOfWork.Coupons.GetAllAsync();

            return coupons.Select(MapToListDto).ToList();
        }

        public async Task<CouponDetailsDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
                return null;

            var usages = await _unitOfWork.CouponUsages
                .GetByCouponIdAsync(id);

            return new CouponDetailsDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaximumDiscountAmount = coupon.MaximumDiscountAmount,
                UsageLimit = coupon.UsageLimit,
                UsageCount = coupon.UsageCount,
                StartsAt = coupon.StartsAt,
                ExpiresAt = coupon.ExpiresAt,
                IsActive = coupon.IsActive,
                CreatedAt = coupon.CreatedAt,
                UpdatedAt = coupon.UpdatedAt,

                Usages = usages.Select(x => new CouponUsageDto
                {
                    Id = x.Id,
                    UsedByUserId = x.UsedByUserId,
                    OrderId = x.OrderId,
                    DiscountAmount = x.DiscountAmount,
                    UsedAt = x.UsedAt
                }).ToList()
            };
        }

        public async Task<CouponDto> CreateAsync(CouponDto dto)
        {
            ValidateCoupon(dto);

            var normalizedCode = NormalizeCode(dto.Code);

            var existingCoupon =
                await _unitOfWork.Coupons.GetByCodeAsync(normalizedCode);

            if (existingCoupon != null)
                throw new InvalidOperationException(
                    "A coupon with this code already exists.");

            var coupon = new Models.Coupon
            {
                Code = normalizedCode,
                Description = NormalizeDescription(dto.Description),
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinimumOrderAmount = dto.MinimumOrderAmount,
                MaximumDiscountAmount = dto.MaximumDiscountAmount,
                UsageLimit = dto.UsageLimit,
                UsageCount = 0,
                StartsAt = NormalizeDate(dto.StartsAt),
                ExpiresAt = NormalizeDate(dto.ExpiresAt),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return MapToFormDto(coupon);
        }

        public async Task<CouponDto?> UpdateAsync(int id, CouponDto dto)
        {
            if (id <= 0)
                return null;

            ValidateCoupon(dto);
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
                return null;

            var normalizedCode = NormalizeCode(dto.Code);

            var existingCoupon =
                await _unitOfWork.Coupons.GetByCodeAsync(normalizedCode);

            if (existingCoupon != null && existingCoupon.Id != id)
                throw new InvalidOperationException(
                    "A coupon with this code already exists.");

            if (dto.UsageLimit.HasValue &&
                dto.UsageLimit.Value < coupon.UsageCount)
            {
                throw new InvalidOperationException(
                    "Usage limit cannot be less than the current usage count.");
            }

            coupon.Code = normalizedCode;
            coupon.Description = NormalizeDescription(dto.Description);
            coupon.DiscountType = dto.DiscountType;
            coupon.DiscountValue = dto.DiscountValue;
            coupon.MinimumOrderAmount = dto.MinimumOrderAmount;
            coupon.MaximumDiscountAmount = dto.MaximumDiscountAmount;
            coupon.UsageLimit = dto.UsageLimit;
            coupon.StartsAt = NormalizeDate(dto.StartsAt);
            coupon.ExpiresAt = NormalizeDate(dto.ExpiresAt);
            coupon.IsActive = dto.IsActive;
            coupon.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Coupons.Update(coupon);

            await _unitOfWork.SaveChangesAsync();

            return MapToFormDto(coupon);
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            if (id <= 0)
                return false;

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
                return false;

            coupon.IsActive = !coupon.IsActive;
            coupon.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Coupons.Update(coupon);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static void ValidateCoupon(CouponDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new InvalidOperationException(
                    "Coupon code is required.");

            if (!Enum.IsDefined(typeof(Veltrix.Enums.DiscountType),
                    dto.DiscountType))
            {
                throw new InvalidOperationException(
                    "Invalid discount type.");
            }

            if (dto.DiscountValue <= 0)
                throw new InvalidOperationException(
                    "Discount value must be greater than zero.");

            if (dto.MinimumOrderAmount < 0)
                throw new InvalidOperationException(
                    "Minimum order amount cannot be negative.");

            if (dto.MaximumDiscountAmount < 0)
                throw new InvalidOperationException(
                    "Maximum discount amount cannot be negative.");

            if (dto.StartsAt == default || dto.ExpiresAt == default)
                throw new InvalidOperationException(
                    "Start date and expiry date are required.");

            if (NormalizeDate(dto.ExpiresAt) <= NormalizeDate(dto.StartsAt))
                throw new InvalidOperationException(
                    "Expiry date must be later than start date.");
        }

        private static string NormalizeCode(string code)
        {
            return code.Trim().ToUpperInvariant();
        }

        private static string? NormalizeDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        private static DateTime NormalizeDate(DateTime date)
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }

        private static CouponListDto MapToListDto(Models.Coupon coupon)
        {
            return new CouponListDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                UsageCount = coupon.UsageCount,
                UsageLimit = coupon.UsageLimit,
                StartsAt = coupon.StartsAt,
                ExpiresAt = coupon.ExpiresAt,
                IsActive = coupon.IsActive
            };
        }

        private static CouponDto MapToFormDto(Models.Coupon coupon)
        {
            return new CouponDto
            {
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaximumDiscountAmount = coupon.MaximumDiscountAmount,
                UsageLimit = coupon.UsageLimit,
                StartsAt = coupon.StartsAt,
                ExpiresAt = coupon.ExpiresAt,
                IsActive = coupon.IsActive
            };
        }
    }
}
