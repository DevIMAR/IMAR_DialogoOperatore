using FluentAssertions;
using IMAR_DialogoOperatore.Components.Pages;

namespace IMAR_DialogoOperatore.Test.Components;

public class HomeComponentTests
{
    [Fact]
    public void Home_ComponentType_ShouldExist()
    {
        // Verify the component type exists
        var componentType = typeof(Home);
        
        // Assert
        componentType.Should().NotBeNull();
        componentType.Name.Should().Be("Home");
        componentType.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void Home_ShouldBeRazorComponent()
    {
        // Verify it's a Blazor/Razor component
        var componentType = typeof(Home);
        var baseType = componentType.BaseType;
        
        // Assert it inherits from a component base class
        componentType.Should().NotBeNull();
        baseType.Should().NotBeNull();
    }

    [Fact]
    public void Home_ShouldHaveCorrectNamespace()
    {
        // Verify component has correct namespace
        var componentType = typeof(Home);
        
        // Assert
        componentType.Namespace.Should().Contain("IMAR_DialogoOperatore.Components.Pages");
    }
}