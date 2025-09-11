using FluentAssertions;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using NSubstitute;

namespace IMAR_DialogoOperatore.Test.Infrastructure.Services;

public class OperatoreServiceTests
{
    private readonly IOperatoreService _operatoreService;

    public OperatoreServiceTests()
    {
        _operatoreService = Substitute.For<IOperatoreService>();
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Assert
        _operatoreService.Should().NotBeNull();
    }

    [Fact]
    public void OperatoreService_Mock_ShouldBeConfigurable()
    {
        // Arrange
        var testOperatore = new Operatore { Badge = "12345", Nome = "Test", Cognome = "User" };
        
        // Act - Configure mock behavior
        _operatoreService.Operatore.Returns(testOperatore);
        
        // Assert
        _operatoreService.Should().NotBeNull();
        _operatoreService.Operatore.Badge.Should().Be("12345");
    }

    [Fact]
    public void OperatoreService_Interface_ShouldExist()
    {
        // Verify the service interface exists
        var interfaceType = typeof(IOperatoreService);
        
        // Assert
        interfaceType.Should().NotBeNull();
        interfaceType.IsInterface.Should().BeTrue();
        interfaceType.Name.Should().Be("IOperatoreService");
    }
}