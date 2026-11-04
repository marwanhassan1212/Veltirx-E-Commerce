using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.ProductVariantImage;
using Veltrix.Services.ProductVariantImage;

namespace Veltrix.Controllers
{
    public class ProductVariantImageController : Controller
    {
        private readonly IProductVariantImageService _productVariantImageService;

        public ProductVariantImageController(
            IProductVariantImageService productVariantImageService)
        {
            _productVariantImageService = productVariantImageService;
        }

        // Get Variant Images
        [HttpGet]
        public async Task<IActionResult> Index(int variantId)
        {
            if (variantId <= 0)
                return BadRequest();

            var images = await _productVariantImageService
                .GetByVariantIdAsync(variantId);

            ViewBag.VariantId = variantId;

            return View(images);
        }

        // Create Image - GET
        [HttpGet]
        public IActionResult Create(int variantId)
        {
            if (variantId <= 0)
                return BadRequest();

            ViewBag.VariantId = variantId;

            return View();
        }

        // Create Image - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int variantId,
            CreateProductVariantImageDto dto)
        {
            ViewBag.VariantId = variantId;

            if (variantId <= 0)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var image = await _productVariantImageService
                    .CreateAsync(variantId, dto);

                if (image == null)
                    return NotFound();

                TempData["Success"] =
                    "Variant image uploaded successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { variantId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(dto.Image), ex.Message);
                return View(dto);
            }
        }

        // Edit Image - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productVariantImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            var dto = new UpdateProductVariantImageDto
            {
                AltText = image.AltText,
                DisplayOrder = image.DisplayOrder
            };

            ViewBag.ImageId = image.Id;
            ViewBag.VariantId = image.ProductVariantId;
            ViewBag.ImageUrl = image.ImageUrl;

            return View(dto);
        }

        // Edit Image - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateProductVariantImageDto dto)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productVariantImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            ViewBag.ImageId = image.Id;
            ViewBag.VariantId = image.ProductVariantId;
            ViewBag.ImageUrl = image.ImageUrl;

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result = await _productVariantImageService
                    .UpdateAsync(id, dto);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Variant image updated successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { variantId = image.ProductVariantId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(dto.Image), ex.Message);
                return View(dto);
            }
        }

        // Set Primary Image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimary(int id)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productVariantImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            var result = await _productVariantImageService
                .SetPrimaryAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "Primary variant image updated successfully.";

            return RedirectToAction(
                nameof(Index),
                new { variantId = image.ProductVariantId });
        }

        // Delete Image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productVariantImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            var result = await _productVariantImageService.DeleteAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "Variant image deleted successfully.";

            return RedirectToAction(
                nameof(Index),
                new { variantId = image.ProductVariantId });
        }
    }
}