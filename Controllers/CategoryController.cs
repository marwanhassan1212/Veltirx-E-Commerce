using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Veltrix.DTOs.CategoryDTO;
using Veltrix.Services.Category;

namespace Veltrix.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        // Get All Categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllAsync();
            return View(categories);
        }

        // Get Category Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await categoryService.GetByIdAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Create Category -> GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create Category -> POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await categoryService.CreateAsync(dto);
                TempData["Success"] = "Category Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch(InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }

        }

        // Edit Category -> GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await categoryService.GetByIdAsync(id);
            if(category == null)
            {
                return NotFound();
            }

            var updateCategory = new UpdateCategoryDto
            {
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId
            };

            ViewBag.CategoryId = id;
            return View(updateCategory);
        }

        // Edit Category -> POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id , UpdateCategoryDto dto)
        {
            ViewBag.CategoryId = id;
            if (!ModelState.IsValid)
                return View(dto);
            try
            {
                var category = await categoryService.UpdateAsync(id , dto);
                if (category == null)
                    return NotFound();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch(InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }


    }
}
