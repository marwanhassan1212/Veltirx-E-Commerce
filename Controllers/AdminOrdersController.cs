
using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.OrderDTO;
using Veltrix.Repositories.Order;
using Veltrix.Services.Order;
using Veltrix.Services.OrderStatusHistory;
using Veltrix.UnitOfWork;

namespace Veltrix.Controllers
{
    public class AdminOrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderQueryService _orderQueryService;
        private readonly IOrderStatusHistoryService _orderStatusHistoryService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminOrdersController(IOrderService orderService, IOrderQueryService orderQueryService,
            IOrderStatusHistoryService orderStatusHistoryService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _orderQueryService = orderQueryService;
            _orderStatusHistoryService = orderStatusHistoryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(OrderFilterParameters filters)
        {
            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);

            var result = await _orderQueryService.GetPagedOrdersAsync(filters);

            ViewBag.Filters = filters;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(
                result.TotalCount / (double)filters.PageSize);

            return View(result.Items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order == null)
                return NotFound();

            var history = await _orderStatusHistoryService
                .GetByOrderIdAsync(id);

            ViewBag.StatusHistory = history;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int orderId,
            UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid order status.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(
                    orderId, dto);

                if (result == null)
                    return NotFound();

                TempData["Success"] = "Order status updated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}
