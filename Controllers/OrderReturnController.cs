
using Microsoft.AspNetCore.Mvc;
using Veltrix.Repositories.OrderReturn;
using Veltrix.Services.OrderReturn;
using Veltrix.Services.OrderReturnItem;

namespace Veltrix.Controllers
{
    public class OrderReturnController : Controller
    {
        private readonly IOrderReturnService _orderReturnService;
        private readonly IOrderReturnItemService _orderReturnItemService;

        public OrderReturnController(IOrderReturnService orderReturnService, IOrderReturnItemService orderReturnItemService)
        {
            _orderReturnService = orderReturnService;
            _orderReturnItemService = orderReturnItemService;
        }

        // Display all return requests with filtering and pagination
        public async Task<IActionResult> Index(OrderReturnFilterParameters filters)

        {
            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);
            var result = await _orderReturnService.GetPagedAsync(filters);
            ViewBag.Filters = filters;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)filters.PageSize);
            return View(result.Items);
        }

        // Display returns belonging to a specific order
        public async Task<IActionResult> ByOrder(int orderId)
        {
            if (orderId <= 0)
                return BadRequest();

            var returns = await _orderReturnService.GetByOrderIdAsync(orderId);
            return View(returns);
        }

        // Display return details and its items
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var orderReturn = await _orderReturnService.GetByIdAsync(id);

            if (orderReturn == null)
                return NotFound();

            var items = await _orderReturnItemService.GetByReturnIdAsync(id);
            ViewBag.ReturnItems = items;
            return View(orderReturn);
        }

        // Find a return using its return number
        public async Task<IActionResult> Search(string returnNumber)
        {
            if (string.IsNullOrWhiteSpace(returnNumber))
                return BadRequest();
            var orderReturn = await _orderReturnService.GetByReturnNumberAsync(returnNumber);
            if (orderReturn == null)
                return NotFound();
            return RedirectToAction(nameof(Details), new { id = orderReturn.Id });
        }
    }
}
