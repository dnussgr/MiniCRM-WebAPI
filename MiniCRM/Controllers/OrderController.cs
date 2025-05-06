using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Data;
using MiniCRM.Dtos;
using MiniCRM.Models;

namespace MiniCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(AppDbContext context, IMapper mapper, ILogger<OrderController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
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

            var orderDtos = _mapper.Map<List<OrderDto>>(orders);

            _logger.LogInformation("Retrieved {count} orders (includeCanceled={includeCanceled})", orders.Count, includeCanceled);

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
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Order {OrderId} retrieved", id);
            return Ok(order);
        }


        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="order">The order to create</param>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            // Check if customer exists
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == createOrderDto.CustomerId && !c.IsDeleted);

            if (!customerExists)
            {
                _logger.LogWarning("Attempt to create order for non-existent or deleted customer {CustomerId}", createOrderDto.CustomerId);
                return BadRequest($"Customer with ID {createOrderDto.CustomerId} does not exist or is deleted.");
            }
                

            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new order with ID {OrderId} by customer {CustomerId}", order.Id, order.CustomerId);

            var orderDto = _mapper.Map<OrderDto>(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }


        /// <summary>
        /// Updates an existing order
        /// </summary>
        /// <param name="id">Id of order to update</param>
        /// <param name="updatedOrder">The updated order data</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Attempt to update non-existent order {OrderId}", id);
                return NotFound();

            }

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == updateOrderDto.CustomerId && !c.IsDeleted);

            if (!customerExists)
            {
                _logger.LogWarning("Attempt to update order of customer with ID {CustomerId} that does not exist", updateOrderDto.CustomerId);
                return BadRequest($"Customer with ID {updateOrderDto.CustomerId} does not exist or is deleted.");
            }
                

            _mapper.Map(updateOrderDto, order);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} successfully updated", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Attempt to cancel non-existent order {OrderId}", id);
                return NotFound();
            }

            if (order.IsCanceled)
            {
                _logger.LogWarning("Attempt to cancel order {OrderId} that has already been canceled", id);
                return BadRequest("Order is already canceled.");
            }
                

            order.IsCanceled = true;
            order.CanceledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully canceled order {OrderId}", id);

            return NoContent();
        }
    }
}
