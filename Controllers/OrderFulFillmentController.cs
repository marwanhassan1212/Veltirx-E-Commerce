
using Microsoft.AspNetCore.Mvc;
using Veltrix.Repositories.OrderFulfillment;
using Veltrix.Services.FulfillmentItem;
using Veltrix.Services.OrderFulfillment;

namespace Veltrix.Controllers
{
    public class OrderFulfillmentController : Controller
    {
        private readonly IOrderFulfillmentService _fulfillmentService;
        private readonly IFulfillmentItemService _fulfillmentItemService;

        public OrderFulfillmentController(IOrderFulfillmentService fulfillmentService, IFulfillmentItemService fulfillmentItemService)
        {
            _fulfillmentService = fulfillmentService;
            _fulfillmentItemService = fulfillmentItemService;
        }


        public async Task<IActionResult> Index(OrderFulfillmentFilterParameters filters)
        {
            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);
            var result = await _fulfillmentService.GetPagedAsync(filters);
            ViewBag.Filters = filters;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)filters.PageSize);
            return View(result.Items);
        }


        // Display fulfillments belonging to a specific order
        public async Task<IActionResult> ByOrder(int orderId)
        {
            if (orderId <= 0)
                return BadRequest();

            var fulfillments = await _fulfillmentService.GetByOrderIdAsync(orderId);
            return View(fulfillments);
        }

        // Display one fulfillment with its items
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var fulfillment = await _fulfillmentService.GetByIdAsync(id);

            if (fulfillment == null)
                return NotFound();

            var items = await _fulfillmentItemService.GetByFulfillmentIdAsync(id);
            ViewBag.FulfillmentItems = items;

            return View(fulfillment);
        }

        // Find fulfillment using its fulfillment number
        public async Task<IActionResult> Search(string fulfillmentNumber)
        {
            if (string.IsNullOrWhiteSpace(fulfillmentNumber))
                return BadRequest();

            var fulfillment = await _fulfillmentService.GetByFulfillmentNumberAsync(fulfillmentNumber);
            if (fulfillment == null)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id = fulfillment.Id });
        }
    }
}
