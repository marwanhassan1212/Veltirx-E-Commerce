
using Veltrix.DTOs.CouponDTO;

namespace Veltrix.Services.Coupon
{
    public interface ICouponService
    {
        Task<IReadOnlyList<CouponListDto>> GetAllAsync();

        Task<CouponDetailsDto?> GetByIdAsync(int id);

        Task<CouponDto> CreateAsync(CouponDto dto);

        Task<CouponDto?> UpdateAsync(int id, CouponDto dto);

        Task<bool> ToggleStatusAsync(int id);
    }
}
