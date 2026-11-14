using Microsoft.AspNetCore.Mvc;
using Veltrix.Repositories.Refund;
using Veltrix.Services.Refund;

namespace Veltrix.Controllers
{
    public class RefundController : Controller
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }
        public async Task<IActionResult> Index(RefundFilterParameters filters)
        {
            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);
            var result = await _refundService.GetPagedAsync(filters);
            ViewBag.Filters = filters;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)filters.PageSize);
            return View(result.Items);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var refund = await _refundService.GetByIdAsync(id);

            if (refund == null)
                return NotFound();

            return View(refund);
        }

        public async Task<IActionResult> ByReturn(int returnId)
        {
            if (returnId <= 0)
                return BadRequest();

            var refunds = await _refundService.GetByReturnIdAsync(returnId);
            ViewBag.ReturnId = returnId;
            return View(refunds);
        }

        public async Task<IActionResult> Search(string refundNumber)
        {
            if (string.IsNullOrWhiteSpace(refundNumber))
                return BadRequest();

            var refund = await _refundService.GetByRefundNumberAsync(refundNumber);
            if (refund == null)
                return NotFound();
            return RedirectToAction(nameof(Details), new { id = refund.Id });
        }
    }
}
