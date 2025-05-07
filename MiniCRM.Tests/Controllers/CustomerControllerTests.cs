using Xunit;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Controllers;
using MiniCRM.Data;
using MiniCRM.Dtos;
using MiniCRM.Models;
using MiniCRM.Profiles;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace MiniCRM.Tests.Controllers
{
    public class CustomerControllerTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            // Prepare InMemory Database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Customers")
                .Options;
            _context = new AppDbContext(options);

            // Load AutoMapper profile
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            // Create Controller
            _controller = new CustomerController(_context, _mapper, NullLogger<CustomerController>.Instance);           
        }

        [Fact]
        public async Task CreateCustomer_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newCustomer = new CreateCustomerDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "text@example.com",
                PhoneNumber = "+1234567890"
            };

            // Act
            var result = await _controller.CreateCustomer(newCustomer);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<CustomerDto>(createdResult.Value);

            Assert.Equal("Test", returnValue.FirstName);
            Assert.Equal("User", returnValue.LastName);
        }
    }
}
