using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Data;
using MiniCRM.Models;

namespace MiniCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders([FromQuery] bool includeCanceled = false)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .AsQueryable();

            if (!includeCanceled)
                query = query.Where(o => !o.IsCanceled);

            var orders = await query.ToListAsync();

            return Ok(orders);
        }


        /// <summary>
        /// Gets order by its ID
        /// </summary>
        /// <param name="id">Order ID</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }


        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="order">The order to create</param>
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            // Check if customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == order.CustomerId && !c.IsDeleted);
            if (!customerExists)
                return BadRequest($"Customer with ID {order.CustomerId} does not exist.");

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }


        /// <summary>
        /// Updates an existing order
        /// </summary>
        /// <param name="id">Id of order to update</param>
        /// <param name="updatedOrder">The updated order data</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order updatedOrder)
        {
            if (id != updatedOrder.Id)
                return BadRequest("ID mismatch.");

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            // Update order
            order.ProductName = updatedOrder.ProductName;
            order.Quantity = updatedOrder.Quantity;
            order.TotalPrice = updatedOrder.TotalPrice;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            if (order.IsCanceled)
                return BadRequest("Order is already canceled.");

            order.IsCanceled = true;
            order.CanceledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
