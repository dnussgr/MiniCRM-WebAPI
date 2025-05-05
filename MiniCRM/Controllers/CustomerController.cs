using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Data;
using MiniCRM.Models;

namespace MiniCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
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



        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            return customer;
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            if (id != updatedCustomer.Id)
                return BadRequest();

            _context.Entry(updatedCustomer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                    return NotFound();

                throw;
            }

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


        private bool CustomerExists(int id) => _context.Customers.Any(e => e.Id == id);
    }
}
