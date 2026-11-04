using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.CategoryDTO;
using Veltrix.Services.Category;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veltrix.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        private async Task PopulateParentCategoriesAsync(int? selectedParentId = null, int? currentCategoryId = null)
        {
            var categories = await _categoryService.GetAllAsync();

            var availableCategories = categories
                .Where(c => !currentCategoryId.HasValue ||
                            c.Id != currentCategoryId.Value)
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.ParentCategories = new SelectList(
                availableCategories,
                "Id",
                "Name",
                selectedParentId
            );
        }

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search , string? status,
            int page = 1 , int pageSize = 10)
        {
            var categories = await _categoryService.GetPagedCategoriesAsync(search,
                status, page, pageSize);
            var statistics = await _categoryService.GetStatisticsAsync();
            ViewBag.Statistics = statistics;
            ViewBag.Search = search;
            ViewBag.Status = status ?? "all";
            ViewBag.PageSize = categories.PageSize;
            return View(categories);

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateParentCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesAsync(dto.ParentCategoryId);
                return View(dto);
            }

            try
            {
                await _categoryService.CreateAsync(dto);

                TempData["Success"] = "Category Added Successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await PopulateParentCategoriesAsync(dto.ParentCategoryId);
                return View(dto);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Category image upload failed.");

                ModelState.AddModelError(
                    string.Empty,
                    "Could not upload the image. Please try again.");

                await PopulateParentCategoriesAsync(dto.ParentCategoryId);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            var updateCategory = new UpdateCategoryDto
            {
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId
            };

            ViewBag.CategoryId = id;
            ViewBag.CurrentImageUrl = category.ImageUrl;

            await PopulateParentCategoriesAsync(
                updateCategory.ParentCategoryId,
                id
            );
            return View(updateCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto dto)
        {
            ViewBag.CategoryId = id;

            var existingCategory = await _categoryService.GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound();

            ViewBag.CurrentImageUrl = existingCategory.ImageUrl;

            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesAsync(dto.ParentCategoryId, id);
                return View(dto);
            }

            try
            {
                var category = await _categoryService.UpdateAsync(id, dto);

                if (category == null)
                    return NotFound();

                TempData["Success"] = "Category Updated Successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await PopulateParentCategoriesAsync(dto.ParentCategoryId, id);
                return View(dto);
            }
            catch (IOException ex)
            {
                _logger.LogError(
                    ex,
                    "Category image update failed for Category ID {CategoryId}",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Could not upload the image. Please try again.");

                await PopulateParentCategoriesAsync(dto.ParentCategoryId, id);
                return View(dto);
            }
        }
    }
}
