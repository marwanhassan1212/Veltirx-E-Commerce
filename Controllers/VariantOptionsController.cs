using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.VariantOptionsDTO;
using Veltrix.Services.VariantOptions;

namespace Veltrix.Controllers
{
    public class VariantOptionsController : Controller
    {
        private readonly IVariantOptionsService _variantOptionsService;

        public VariantOptionsController(
            IVariantOptionsService variantOptionsService)
        {
            _variantOptionsService = variantOptionsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            var options = await _variantOptionsService.GetByProductIdAsync(productId);

            ViewBag.ProductId = productId;

            return View(options);
        }

        [HttpGet]
        public IActionResult Create(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            return View(new CreateVariantOptionsDto
            {
                ProductId = productId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVariantOptionsDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _variantOptionsService.CreateAsync(dto);

                TempData["Success"] = "Option group created successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { productId = dto.ProductId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int productId)
        {
            if (id <= 0 || productId <= 0)
                return BadRequest();

            var options = await _variantOptionsService.GetByProductIdAsync(productId);
            var option = options.FirstOrDefault(x => x.Id == id);

            if (option == null)
                return NotFound();

            var dto = new UpdateVariantOptionsDto
            {
                Name = option.Name,
            };

            ViewBag.ProductId = productId;
            ViewBag.OptionId = id;

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            int productId,
            UpdateVariantOptionsDto dto)
        {
            if (id <= 0 || productId <= 0)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = productId;
                ViewBag.OptionId = id;
                return View(dto);
            }

            try
            {
                var options = await _variantOptionsService
                    .GetByProductIdAsync(productId);

                var existingOption = options.FirstOrDefault(x => x.Id == id);

                if (existingOption == null)
                    return NotFound();


                var result = await _variantOptionsService.UpdateAsync(id, dto);

                if (result == null)
                    return NotFound();

                TempData["Success"] = "Option group updated successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { productId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.ProductId = productId;
                ViewBag.OptionId = id;

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            try
            {
                var deleted = await _variantOptionsService.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["Success"] = "Option group deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(
                nameof(Index),
                new { productId });
        }
    }
}