
using Microsoft.AspNetCore.Mvc;
using Veltrix.Services.Shipment;
using Veltrix.Services.ShipmentTrackingEvent;

namespace Veltrix.Controllers
{
    public class ShipmentController : Controller
    {
        private readonly IShipmentService _shipmentService;
        private readonly IShipmentTrackingEventService _trackingEventService;

        public ShipmentController(
            IShipmentService shipmentService,
            IShipmentTrackingEventService trackingEventService)
        {
            _shipmentService = shipmentService;
            _trackingEventService = trackingEventService;
        }

        // Get shipments belonging to a specific order
        public async Task<IActionResult> ByOrder(int orderId)
        {
            if (orderId <= 0)
                return BadRequest();

            var shipments = await _shipmentService.GetByOrderIdAsync(orderId);
            return View(shipments);
        }

        // Get shipment details and its tracking history
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var shipment = await _shipmentService.GetByIdAsync(id);

            if (shipment == null)
                return NotFound();

            var trackingEvents = await _trackingEventService.GetByShipmentIdAsync(id);
            ViewBag.TrackingEvents = trackingEvents;
            return View(shipment);
        }

        // Track shipment using its tracking number
        public async Task<IActionResult> Track(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest();

            var shipment = await _shipmentService.GetByTrackingNumberAsync(trackingNumber);

            if (shipment == null)
                return NotFound();

            var trackingEvents = await _trackingEventService.GetByShipmentIdAsync(shipment.Id);
            ViewBag.TrackingEvents = trackingEvents;
            return View(shipment);
        }
    }
}
