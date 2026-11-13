
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Veltrix.Services.ShippingSettings;

namespace Veltrix.Controllers
{
    public class ShippingSettingsController : Controller
    {
        private readonly IShippingSettingsService _shippingSettingsService;

        public ShippingSettingsController(IShippingSettingsService shippingSettingsService)
        {
            _shippingSettingsService = shippingSettingsService;
        }

        // Display shipping settings
        public async Task<IActionResult> Index()
        {
            var settings = await _shippingSettingsService.GetAllAsync();
            var activeSetting = await _shippingSettingsService.GetActiveAsync();
            ViewBag.ActiveSetting = activeSetting;
            return View(settings);
        }

        // Display one shipping setting
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();
            var setting = await _shippingSettingsService.GetByIdAsync(id);
            if (setting == null)
                return NotFound();
            return View(setting);
        }

        // Update the active shipping fee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShippingFee(decimal shippingFee)
        {
            if (shippingFee < 0)
            {
                TempData["Error"] = "Shipping fee cannot be negative.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _shippingSettingsService.UpdateShippingFeeAsync(shippingFee, userId);
                TempData["Success"] = "Shipping fee updated successfully.";

            }
            catch (ArgumentOutOfRangeException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
