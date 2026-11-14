
using Microsoft.AspNetCore.Mvc;
using Veltrix.DTOs.InventoryDTO;
using Veltrix.Services.Inventory;

namespace Veltrix.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: Inventory
        public async Task<IActionResult> Index([FromQuery] InventoryFilterDto filter)
        {
            try
            {
                var result = await _inventoryService.GetFilteredAsync(filter);
                return View(result);
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inventory/Details/
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var inventory = await _inventoryService.GetByIdAsync(id);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // GET: Inventory/Transactions/5
        public async Task<IActionResult> Transactions(int id)
        {
            if (id <= 0)
                return BadRequest();

            var inventory = await _inventoryService.GetByIdAsync(id);

            if (inventory == null)
                return NotFound();

            var transactions = await _inventoryService.GetTransactionsAsync(id);
            ViewBag.Inventory = inventory;
            return View(transactions);
        }

        // POST: Inventory/UpdateStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, UpdateInventoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter valid stock information.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _inventoryService.UpdateStockAsync(id, dto);
                TempData["Success"] = "Inventory updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Inventory/TransactionsByOrder/5
        public async Task<IActionResult> TransactionsByOrder(int orderId)
        {
            if (orderId <= 0)
                return BadRequest();

            var transactions = await _inventoryService.GetTransactionsByOrderIdAsync(orderId);


            ViewBag.OrderId = orderId;

            return View(transactions);
        }
    }
}
