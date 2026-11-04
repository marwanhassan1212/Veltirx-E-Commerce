using Microsoft.AspNetCore.Mvc;
using Veltrix.Services.Product;

namespace Veltrix.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductService _productService;

        public StoreController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("/Store/Details/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest();

            var product = await _productService.GetStorefrontProductAsync(slug);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}