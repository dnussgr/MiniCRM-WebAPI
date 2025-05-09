using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniCRM.Controllers;
using MiniCRM.Data;
using MiniCRM.Dtos;
using MiniCRM.Models;
using MiniCRM.Profiles;

namespace MiniCRM.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            //Prepare IMemory Database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Orders_{Guid.NewGuid()}")
                .Options;
            _context = new AppDbContext(options);

            // Load AutoMapper profile
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            // Create Controller
            _controller = new OrderController(_context, _mapper, NullLogger<OrderController>.Instance);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Order",
                LastName = "User",
                Email = "order@example.com",
                PhoneNumber = "+00000",
                CreatedAt = DateTime.UtcNow
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();


            var newOrder = new CreateOrderDto
            {
                CustomerId = customer.Id,
                ProductName = "Test order",
                Quantity = 1,
                TotalPrice = 0.99M
            };

            // Act
            var result = await _controller.CreateOrder(newOrder);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<OrderDto>(createdResult.Value);

            Assert.Equal(1, returnValue.CustomerId);
            Assert.Equal("Test order", returnValue.ProductName);
            Assert.Equal(1, returnValue.Quantity);
            Assert.Equal(0.99M, returnValue.TotalPrice);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequestIfCustomerIsDeleted()
        {
            //Arrange
            var deletedCustomer = new Customer
            {
                FirstName = "Ghost",
                LastName = "Customer",
                Email = "ghost@example.com",
                PhoneNumber = "+00000",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };
            _context.Customers.Add(deletedCustomer);
            await _context.SaveChangesAsync();

            var order = new CreateOrderDto
            {
                CustomerId = deletedCustomer.Id,
                ProductName = "Ghost Order",
                Quantity = 1,
                TotalPrice = 10.0M
            };

            // Act
            var result = await _controller.CreateOrder(order);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal($"Customer with ID {order.CustomerId} does not exist or is deleted.", badRequest.Value);            
        }
    }
}
