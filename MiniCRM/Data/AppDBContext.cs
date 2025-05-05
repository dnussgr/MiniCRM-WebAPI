using Microsoft.EntityFrameworkCore;
using MiniCRM.Models;

namespace MiniCRM.Data
{
    /// <summary>
    /// Entity Framework database context for the CRM system
    /// </summary>
    public class AppDBContext : DbContext
    {
        /// <summary>
        /// Constructor that accepts DBContext options
        /// </summary>
        /// <param name="options"></param>
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }


        /// <summary>
        /// Represents the Customers table in the database
        /// </summary>
        public DbSet<Customer> Customers => Set<Customer>();

        /// <summary>
        /// Represents the Orders table in the database
        /// </summary>
        public DbSet<Order> Orders => Set<Order>();
    }
}
