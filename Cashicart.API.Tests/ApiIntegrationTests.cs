using Cashicart.API.Controllers;
using Cashicart.Application.Features.Products.Commands;
using Cashicart.Common.Interfaces;
using Cashicart.Infrastructure.Data;
using Cashicart.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Cashicart.API.Tests
{
    public class ApiIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;
        private readonly CashicartDbContext _dbContext;
        private readonly ILogger<ProductsController> _logger;

        public ApiIntegrationTests()
        {
            // Use the absolute path to the application’s build output
            string appBasePath = @"F:\codeFirst\Cashicart\Cashicart.API\bin\Debug\net8.0";

            if (!Directory.Exists(appBasePath))
            {
                throw new DirectoryNotFoundException($"The content root path '{appBasePath}' does not exist. Please build the Cashicart.API project first.");
            }

            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(appBasePath)
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CashicartDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<CashicartDbContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase" + Guid.NewGuid().ToString()));

                    // Add routing, controllers, and use the application’s assembly
                    services.AddRouting();
                    services.AddControllers()
                            .AddApplicationPart(typeof(ProductsController).Assembly);

                    // Add MediatR and validators
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
                    services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);

                    // Add logging and IUnitOfWork
                    services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
                    services.AddScoped<IUnitOfWork, UnitOfWork>(); // Replace UnitOfWork with your implementation
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                }));

            _client = _server.CreateClient();
            var serviceProvider = _server.Services.CreateScope().ServiceProvider;
            _dbContext = serviceProvider.GetRequiredService<CashicartDbContext>();
            _logger = serviceProvider.GetRequiredService<ILogger<ProductsController>>();

            // Debug output to verify setup
            Console.WriteLine($"Content root: {appBasePath}");
            Console.WriteLine($"Base address: {_client.BaseAddress}");
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreated()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                SKU = "TST002",
                Price = 200m,
                StockQuantity = 15,
                CategoryId = Guid.NewGuid()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", command);

            // Assert and debug
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}, Headers: {string.Join(", ", response.Headers)}, Content: {responseContent}");

            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent) ?? new Dictionary<string, object>();
            Console.WriteLine($"Deserialized Result: {JsonSerializer.Serialize(result)}");

            Assert.True(result.ContainsKey("productId"), $"Expected 'productId' key, but found keys: {string.Join(", ", result.Keys)}");
            Assert.True(result["productId"] != null, "productId value is null"); // Replaced Assert.NotNull with Assert.True
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
            _server.Dispose();
        }
    }
}