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

        [Fact]
        public async Task UpdateCustomer_UpdatesCustomerSuccessfully()
        {
            // Arrange
            var original = new Customer
            {
                FirstName = "Original",
                LastName = "User",
                Email = "original@example.com",
                PhoneNumber = "+1234567890",
                CreatedAt = DateTime.UtcNow
            };
            _context.Customers.Add(original);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateCustomerDto
            {
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                PhoneNumber = "+0987654321"
            };

            // Act
            var result = await _controller.UpdateCustomer(original.Id, updateDto);

            // Assert
            var updated = await _context.Customers.FindAsync(original.Id);
            Assert.Equal("Updated", updated!.FirstName);
            Assert.Equal($"{updated.LastName}", updated!.LastName);
            Assert.Equal("updated@example.com", updated.Email);
            Assert.Equal("+0987654321", updated!.PhoneNumber);
        }

        [Fact]
        public async Task UpdateCustomer_IsDeleted_ReturnsBadRequest()
        {
            // Arrange
            var deletedCustomer = new Customer
            {
                FirstName = "Deleted",
                LastName = "Guy",
                Email = "gone@example.com",
                PhoneNumber = "+000000",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };
            _context.Customers.Add(deletedCustomer);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateCustomerDto
            {
                FirstName = "ShouldNot",
                LastName = "Work",
                Email = "failed@example.com",
                PhoneNumber = "+999999999"
            };

            // Act
            var result = await _controller.UpdateCustomer(deletedCustomer.Id, updateDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer is deleted and cannot be updated.", badRequest.Value);
        }

        [Fact]
        public async Task DeleteCustomer_DeletesCorrectly()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "To",
                LastName = "Delete",
                Email = "delete@example.com",
                PhoneNumber = "+987654",
                CreatedAt = DateTime.UtcNow
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteCustomer(customer.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var deletedCustomer = await _context.Customers.FindAsync(customer.Id);
            Assert.True(deletedCustomer!.IsDeleted);
            Assert.NotNull(deletedCustomer.DeletedAt);
            Assert.True(deletedCustomer.DeletedAt <= DateTime.UtcNow);
        }
    }
}
