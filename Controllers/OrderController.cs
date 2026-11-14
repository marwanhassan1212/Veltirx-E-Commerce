
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.OrderDTO;
using Veltrix.Services.Order;

namespace Veltrix.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderQueryService _orderQueryService;

        public OrderController(IOrderService orderService, IOrderQueryService orderQueryService)
        {
            _orderService = orderService;
            _orderQueryService = orderQueryService;
        }

        // GET: /Order
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var orders = await _orderQueryService
                .GetCustomerOrdersAsync(userId);
            return View(orders);
        }

        // GET: /Order/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateOrderDto());
        }

        // POST: /Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, dto);
                TempData["SuccessMessage"] = "Your order has been placed successfully.";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var order = await _orderQueryService
                .GetCustomerOrderByIdAsync(userId, id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: /Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, CancelOrderDto dto)


        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide a valid cancellation reason.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                var order = await _orderService.CancelOrderAsync(userId, id, dto);
                if (order == null)
                    return NotFound();

                TempData["SuccessMessage"] = "Your order has been cancelled successfully.";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
