using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.VariantOptionValueDTO;
using Veltrix.Services.VariantOptionValues;

namespace Veltrix.Controllers
{
    public class VariantOptionValuesController : Controller
    {
        private readonly IVariantOptionValueService _valueService;

        public VariantOptionValuesController(
            IVariantOptionValueService valueService)
        {
            _valueService = valueService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int groupId)
        {
            if (groupId <= 0)
                return BadRequest();

            var values = await _valueService.GetByGroupIdAsync(groupId);

            ViewBag.GroupId = groupId;

            return View(values);
        }

        [HttpGet]
        public IActionResult Create(int groupId)
        {
            if (groupId <= 0)
                return BadRequest();

            return View(new CreateVariantOptionValueDto
            {
                VariantOptionId = groupId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVariantOptionValueDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _valueService.CreateAsync(dto);

                TempData["Success"] = "Option value created successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { groupId = dto.VariantOptionId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int groupId)
        {
            var values = await _valueService.GetByGroupIdAsync(groupId);
            var value = values.FirstOrDefault(x => x.Id == id);

            if (value == null)
                return NotFound();

            var dto = new UpdateVariantOptionValueDto
            {
                Value = value.Value,
            };

            ViewBag.GroupId = groupId;
            ViewBag.ValueId = id;

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            int groupId,
            UpdateVariantOptionValueDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GroupId = groupId;
                ViewBag.ValueId = id;
                return View(dto);
            }

            try
            {
                var result = await _valueService.UpdateAsync(id, dto);

                if (result == null)
                    return NotFound();

                TempData["Success"] = "Option value updated successfully.";

                return RedirectToAction(
                    nameof(Index),
                    new { groupId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.GroupId = groupId;
                ViewBag.ValueId = id;

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int groupId)
        {
            try
            {
                var deleted = await _valueService.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["Success"] = "Option value deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(
                nameof(Index),
                new { groupId });
        }
    }
}