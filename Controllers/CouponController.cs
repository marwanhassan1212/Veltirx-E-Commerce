
using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.CouponDTO;
using Veltrix.Enums;
using Veltrix.Services.Coupon;

namespace Veltrix.Controllers
{
    public class CouponsController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // GET: Coupons
        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllAsync();
            return View(coupons);
        }

        // GET: Coupons/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        // GET: Coupons/Create
        [HttpGet]
        public IActionResult Create()
        {
            SetDiscountTypes();

            var model = new CouponDto
            {
                StartsAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsActive = true
            };

            return View(model);
        }

        // POST: Coupons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponDto dto)
        {
            if (!ModelState.IsValid)
            {
                SetDiscountTypes();
                return View(dto);
            }

            try
            {
                var coupon = await _couponService.CreateAsync(dto);
                TempData["SuccessMessage"] = $"Coupon {coupon.Code} created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                SetDiscountTypes();
                return View(dto);
            }
        }

        // GET: Coupons/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            if (coupon == null)
                return NotFound();

            var model = new CouponDto
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

            SetDiscountTypes();
            return View(model);
        }

        // POST: Coupons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponDto dto)
        {
            if (!ModelState.IsValid)
            {
                SetDiscountTypes();
                return View(dto);
            }

            try
            {
                var updatedCoupon =
                    await _couponService.UpdateAsync(id, dto);

                if (updatedCoupon == null)
                    return NotFound();

                TempData["SuccessMessage"] = $"Coupon {updatedCoupon.Code} updated successfully.";


                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                SetDiscountTypes();
                return View(dto);
            }
        }

        // POST: Coupons/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _couponService.ToggleStatusAsync(id);

            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Coupon status updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void SetDiscountTypes()
        {
            ViewBag.DiscountTypes = Enum.GetValues<DiscountType>();
        }
    }
}
