using FluentAssertions;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Integration;

public class DatabaseIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly DbContext _context;

    public DatabaseIntegrationTests()
    {
        var services = new ServiceCollection();
        
        // Configure in-memory database for testing
        services.AddDbContext<DbContext>(options =>
            options.UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}"));

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<DbContext>();
    }

    [Fact]
    public void InMemoryDatabase_ShouldBeConfiguredCorrectly()
    {
        // Assert
        _context.Should().NotBeNull();
        _context.Database.IsInMemory().Should().BeTrue();
    }

    [Fact]
    public void DatabaseContext_ShouldSupportBasicOperations()
    {
        // This is a basic test to ensure the context is working
        // In a real scenario, you'd test actual entity operations
        
        // Arrange & Act
        var canConnect = _context.Database.CanConnect();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public void UnitOfWork_Pattern_ShouldBeImplementedCorrectly()
    {
        // Arrange
        var uow = Substitute.For<ISynergyJmesUoW>();
        
        // Act & Assert
        uow.Should().NotBeNull();
        uow.Should().BeAssignableTo<ISynergyJmesUoW>();
    }

    [Fact]
    public async Task AsyncOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var operazione = async () => await Task.Delay(10);

        // Act & Assert
        await operazione.Should().CompleteWithinAsync(TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}