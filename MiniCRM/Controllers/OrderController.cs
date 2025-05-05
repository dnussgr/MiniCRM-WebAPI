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

        public OrderController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            // Check if customer exists
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == createOrderDto.CustomerId && !c.IsDeleted);

            if (!customerExists)
                return BadRequest($"Customer with ID {createOrderDto.CustomerId} does not exist or is deleted.");

            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

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
                return NotFound();

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == updateOrderDto.CustomerId && !c.IsDeleted);

            if (!customerExists)
                return BadRequest($"Customer with ID {updateOrderDto.CustomerId} does not exist or is deleted.");

            _mapper.Map(updateOrderDto, order);

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
