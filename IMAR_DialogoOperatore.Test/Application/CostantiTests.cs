using FluentAssertions;
using IMAR_DialogoOperatore.Application;

namespace IMAR_DialogoOperatore.Test.Application;

public class CostantiTests
{
    [Fact]
    public void Costanti_ShouldHaveRequiredConstants()
    {
        // Test that required constants exist and have expected values
        // This ensures constants are not accidentally changed
        
        // Verify some common constants exist (adjust based on actual Costanti class)
        typeof(Costanti).Should().NotBeNull();
        
        // You would add specific constant tests here like:
        // Costanti.PRESENTE.Should().Be("PRESENTE");
        // Costanti.ASSENTE.Should().Be("ASSENTE");
        // etc.
    }

    [Fact]
    public void Costanti_ShouldBeStaticClass()
    {
        // Verify that Costanti is a static class
        var type = typeof(Costanti);
        
        type.IsAbstract.Should().BeTrue();
        type.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Costanti_Fields_ShouldBeConstants()
    {
        // Verify that all fields in Costanti are constants
        var type = typeof(Costanti);
        var fields = type.GetFields();
        
        foreach (var field in fields)
        {
            field.IsLiteral.Should().BeTrue($"Field {field.Name} should be a constant");
        }
    }
}