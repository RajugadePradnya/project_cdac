using Microsoft.AspNetCore.Mvc;
using RapidReachApi.Data;
using RapidReachApi.Models;
using System.Threading.Tasks;
using System.Linq;

namespace RapidReachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly RapidReachDbContext _db;

        public ShipmentsController(RapidReachDbContext db)
        {
            _db = db;
        }

        // Endpoint for creating a shipment
        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] Shipment shipment)
        {
            if (shipment == null)
                return BadRequest("Shipment is null");

            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            return Ok(new { shipment.ShipmentId });
        }

        // Endpoint to get all shipments
        [HttpGet]
        public IActionResult GetShipments()
        {
            var shipments = _db.Shipments.ToList();
            return Ok(shipments);
        }

        // ...add more endpoints for updating/deleting shipments as needed
    }
}
