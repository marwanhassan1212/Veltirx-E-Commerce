using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Veltrix.DTOs.ProductDTO;
using Veltrix.Services.Category;
using Veltrix.Services.Product;

namespace Veltrix.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService , ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        private async Task LoadCategoriesAsync()
        {
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
        }

        // Get All Products
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1 , int pageSize = 10
            , string? search  = null , string? status = null)
        {
            var products = await _productService.GetPagedAsync(pageNumber, pageSize,
                search, status);

            ViewBag.Search = search;
            ViewBag.Status = status;
            
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
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View();
        }
        
        // Create Product - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(dto);
            }

            try
            {
                await _productService.CreateAsync(dto);

                TempData["Success"] = "Product created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadCategoriesAsync();
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
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                CategoryId = product.CategoryId
            };

            ViewBag.ProductId = id;
            await LoadCategoriesAsync();
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
            {
                await LoadCategoriesAsync();
                return View(updateProduct);
            }
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
                await LoadCategoriesAsync();
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