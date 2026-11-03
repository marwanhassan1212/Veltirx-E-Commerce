using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.ProductDTO;
using Veltrix.Services.Product;

namespace Veltrix.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Get All Products
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        // Get Specific Product
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // Create Product - GET
 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _productService.CreateAsync(dto);

                TempData["Success"] = "Product created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // Edit Product - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var updateProduct = new UpdateProductDto
            {
                Name = product.Name,
                Slug = product.Slug,
                SKU = product.SKU,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            ViewBag.ProductId = id;

            return View(updateProduct);
        }

        // Edit Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto updateProduct)
        {
            if (id <= 0)
                return BadRequest();

            ViewBag.ProductId = id;

            if (!ModelState.IsValid)
                return View(updateProduct);

            try
            {
                var product = await _productService.UpdateAsync(id, updateProduct);

                if (product == null)
                    return NotFound();

                TempData["Success"] = "Product updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(updateProduct);
            }
        }

        // Publish Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result = await _productService.PublishAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] = "Product published successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Unpublish Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result = await _productService.UnpublishAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] = "Product unpublished successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}