
using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.ProductVariantDTO;
using Veltrix.Services.ProductVariant;

namespace Veltrix.Controllers
{
    public class ProductVariantController : Controller
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(
            IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        private async Task LoadOptionGroupsAsync(int productId)
        {
            ViewBag.OptionGroups =
                await _productVariantService.GetOptionGroupsWithValuesAsync(productId);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int productId)
        {
            if (productId <= 0)
            {
                ViewBag.ProductId = 0;
                return View(Array.Empty<ProductVariantDto>());
            }

            var variants = await _productVariantService
                .GetByProductIdAsync(productId);

            ViewBag.ProductId = productId;

            return View(variants);
        }

        // GET: ProductVariant/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var variant = await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            return View(variant);
        }

        // GET: ProductVariant/Create?productId=1
        [HttpGet]
        public IActionResult Create(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            ViewBag.ProductId = productId;

            return View(new CreateProductVariantDto
            {
                ProductId = productId
            });
        }

        // POST: ProductVariant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVariantDto dto)
        {
            ViewBag.ProductId = dto.ProductId;

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _productVariantService.CreateAsync(dto);

                TempData["Success"] =
                    "Variant created successfully.";

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

        // GET: ProductVariant/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var variant = await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            var dto = new UpdateProductVariantDto
            {
                SKU = variant.SKU,
                Price = variant.Price
            };

            ViewBag.VariantId = id;
            ViewBag.ProductId = variant.ProductId;

            return View(dto);
        }

        // POST: ProductVariant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductVariantDto dto)
        {
            var existingVariant =
                await _productVariantService.GetByIdAsync(id);

            if (existingVariant == null)
                return NotFound();

            ViewBag.VariantId = id;
            ViewBag.ProductId = existingVariant.ProductId;

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var updatedVariant =
                    await _productVariantService.UpdateAsync(id, dto);

                if (updatedVariant == null)
                    return NotFound();

                TempData["Success"] =
                    "Variant updated successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { productId = updatedVariant.ProductId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var variant = await _productVariantService.GetByIdAsync(id);

            if (variant == null)
            {
                TempData["Error"] = "Variant not found.";
                return RedirectToAction(nameof(Index));
            }

            int productId = variant.ProductId;

            try
            {
                var result = await _productVariantService.SetDefaultAsync(id);

                if (result)
                {
                    TempData["Success"] =
                        "Default variant updated successfully.";
                }
                else
                {
                    TempData["Error"] =
                        "Unable to update the default variant. Please try again.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
            }

            return RedirectToAction(nameof(Index),
                new { productId });
        }

        // POST: ProductVariant/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var result =
                    await _productVariantService.ActivateAsync(id);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Variant activated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return await RedirectToVariantIndex(id);
        }

        // POST: ProductVariant/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var result =
                    await _productVariantService.DeactivateAsync(id);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Variant deactivated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return await RedirectToVariantIndex(id);
        }

        // POST: ProductVariant/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var variant =
                await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            int productId = variant.ProductId;

            try
            {
                var result =
                    await _productVariantService.DeleteAsync(id);

                if (!result)
                {
                    TempData["Error"] =
                        "Unable to delete this variant.";
                }
                else
                {
                    TempData["Success"] =
                        "Variant deleted successfully.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(
                nameof(Index),
                new { productId });
        }

        // GET: ProductVariant/SetOptions/5
        [HttpGet]
        public async Task<IActionResult> SetOptions(int id)
        {
            if (id <= 0)
                return BadRequest();

            var variant =
                await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            ViewBag.VariantId = id;
            ViewBag.ProductId = variant.ProductId;
            await LoadOptionGroupsAsync(variant.ProductId);
            return View(new SetVariantOptionsDto());
        }

        // POST: ProductVariant/SetOptions/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetOptions(int id, SetVariantOptionsDto dto)
        {
            var variant =
                await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            ViewBag.VariantId = id;
            ViewBag.ProductId = variant.ProductId;
            await LoadOptionGroupsAsync(variant.ProductId);

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result =
                    await _productVariantService.SetOptionsAsync(id, dto);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Variant options assigned successfully.";

                return await RedirectToVariantIndex(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(dto);
            }
        }

        // GET: ProductVariant/EditOptions/5
        [HttpGet]
        public async Task<IActionResult> EditOptions(int id)
        {
            if (id <= 0)
                return BadRequest();

            var variant =
                await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            ViewBag.VariantId = id;
            ViewBag.ProductId = variant.ProductId;
            await LoadOptionGroupsAsync(variant.ProductId);
            return View(new SetVariantOptionsDto
            {
                VariantOptionValueIds = variant.VariantOptionValueIds
            });
        }

        // POST: ProductVariant/EditOptions/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOptions(int id, SetVariantOptionsDto dto)
        {
            var variant =
                await _productVariantService.GetByIdAsync(id);

            if (variant == null)
                return NotFound();

            ViewBag.VariantId = id;
            ViewBag.ProductId = variant.ProductId;
            await LoadOptionGroupsAsync(variant.ProductId);
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result =
                    await _productVariantService.UpdateOptionsAsync(id, dto);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Variant options updated successfully.";

                return await RedirectToVariantIndex(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(dto);
            }
        }

        // Helper: Redirect to the variants list of the product
        private async Task<IActionResult> RedirectToVariantIndex(int variantId)

        {
            var variant =
                await _productVariantService.GetByIdAsync(variantId);

            if (variant == null)
                return RedirectToAction("Index", "Product");

            return RedirectToAction(
                nameof(Index),
                new { productId = variant.ProductId });
        }
    }
}
