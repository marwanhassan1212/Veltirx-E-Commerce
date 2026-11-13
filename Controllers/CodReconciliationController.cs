
using Microsoft.AspNetCore.Mvc;
using Veltrix.Repositories.CodReconciliation;
using Veltrix.Services.CodReconciliation;
using Veltrix.Services.CodReconciliationItem;

namespace Veltrix.Controllers
{
    public class CodReconciliationController : Controller
    {
        private readonly ICodReconciliationService _reconciliationService;
        private readonly ICodReconciliationItemService _reconciliationItemService;

        public CodReconciliationController(ICodReconciliationService reconciliationService, ICodReconciliationItemService reconciliationItemService)
        {
            _reconciliationService = reconciliationService;
            _reconciliationItemService = reconciliationItemService;
        }

        // Display all COD reconciliations with filtering and pagination
        public async Task<IActionResult> Index(CodReconciliationFilterParameters filters)
        {
            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);
            var result = await _reconciliationService.GetPagedAsync(filters);
            ViewBag.Filters = filters;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)filters.PageSize);
            return View(result.Items);
        }

        // Display reconciliation details and its items
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var reconciliation = await _reconciliationService.GetByIdAsync(id);

            if (reconciliation == null)
                return NotFound();

            var items = await _reconciliationItemService.GetByReconciliationIdAsync(id);
            ViewBag.ReconciliationItems = items;
            return View(reconciliation);
        }

        // Find reconciliation using its reconciliation number
        public async Task<IActionResult> Search(string reconciliationNumber)
        {
            if (string.IsNullOrWhiteSpace(reconciliationNumber))
                return BadRequest();

            var reconciliation = await _reconciliationService.GetByReconciliationNumberAsync(reconciliationNumber);
            if (reconciliation == null)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id = reconciliation.Id });
        }
    }
}
