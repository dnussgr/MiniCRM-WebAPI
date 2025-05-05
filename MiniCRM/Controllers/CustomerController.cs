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
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomerController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Gets all customers, optionally including deleted/anonymized ones.
        /// </summary>
        /// <param name="includeDeleted">If true, also includes deleted/anonymized customers.</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers([FromQuery] bool includeDeleted = false)
        {
            var query = _context.Customers.AsQueryable();

            if (!includeDeleted)
                query = query.Where(c => !c.IsDeleted);

            var customers = await query.ToListAsync();
            return Ok(customers);
        }


        /// <summary>
        /// Get a specific customer by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            return customer;
        }


        /// <summary>
        /// Creates a customer
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            var customer = _mapper.Map<Customer>(createCustomerDto);
            customer.CreatedAt = DateTime.UtcNow;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customerDto);
        }


        /// <summary>
        /// Updates an existing customer
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            if (customer.IsDeleted)
                return BadRequest("Customer is deleted and cannot be updated.");

            _mapper.Map(updateCustomerDto, customer);

            await _context.SaveChangesAsync();
            return NoContent();
        }


        /// <summary>
        /// Soft delete a customer
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            if (customer.IsDeleted)
                return BadRequest("Customer is already marked as deleted.");

            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        /// <summary>
        /// Anonymizes a customer's personal data to comply with data protection regulations.
        /// </summary>
        /// <param name="id">The ID of the customer to anonymize.</param>
        [HttpDelete("anonymize/{id}")]
        public async Task<IActionResult> AnonymizeCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            if (customer.IsDeleted)
                return BadRequest("Customer is already deleted.");

            customer.FirstName = "Anonymized";
            customer.LastName = $"User_{customer.Id}";
            customer.Email = $"deleted_{customer.Id}@example.com";
            customer.PhoneNumber = null;
            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
