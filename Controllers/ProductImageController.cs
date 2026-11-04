using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.ProductImageDTO;
using Veltrix.Services.ProductImage;

namespace Veltrix.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _productImageService;

        public ProductImageController(
            IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }
        // Get Product Images
        [HttpGet]
        public async Task<IActionResult> Index(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            var images = await _productImageService
                .GetByProductIdAsync(productId);

            ViewBag.ProductId = productId;

            return View(images);
        }

        // Create Image - GET
        [HttpGet]
        public IActionResult Create(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            ViewBag.ProductId = productId;

            return View();
        }

        // Create Image - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int productId,
            CreateProductImageDto dto)
        {
            ViewBag.ProductId = productId;

            if (productId <= 0)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _productImageService.CreateAsync(productId, dto);

                TempData["Success"] = "Product image uploaded successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { productId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(dto.Image), ex.Message);
                return View(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // Edit Image - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            var dto = new UpdateProductImageDto
            {
                AltText = image.AltText,
                DisplayOrder = image.DisplayOrder
            };

            ViewBag.ImageId = image.Id;
            ViewBag.ProductId = image.ProductId;
            ViewBag.ImageUrl = image.ImageUrl;

            return View(dto);
        }

        // Edit Image - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateProductImageDto dto)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            ViewBag.ImageId = image.Id;
            ViewBag.ProductId = image.ProductId;
            ViewBag.ImageUrl = image.ImageUrl;

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result = await _productImageService.UpdateAsync(id, dto);

                if (!result)
                    return NotFound();

                TempData["Success"] = "Product image updated successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { productId = image.ProductId });
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

            var image = await _productImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            var result = await _productImageService.SetPrimaryAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] = "Primary image updated successfully.";

            return RedirectToAction(
                nameof(Index),
                new { productId = image.ProductId });
        }


        // Delete Image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var image = await _productImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            var result = await _productImageService.DeleteAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] = "Product image deleted successfully.";

            return RedirectToAction(
                nameof(Index),
                new { productId = image.ProductId });
        }

    }
}
