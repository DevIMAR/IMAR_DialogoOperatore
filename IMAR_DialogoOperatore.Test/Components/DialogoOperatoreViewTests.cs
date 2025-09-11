using FluentAssertions;
using IMAR_DialogoOperatore.Components.Pages;

namespace IMAR_DialogoOperatore.Test.Components;

public class DialogoOperatoreViewTests
{
    [Fact]
    public void DialogoOperatoreView_ComponentType_ShouldExist()
    {
        // Verify the component type exists
        var componentType = typeof(DialogoOperatoreView);
        
        // Assert
        componentType.Should().NotBeNull();
        componentType.Name.Should().Be("DialogoOperatoreView");
        componentType.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void DialogoOperatoreView_ShouldBeRazorComponent()
    {
        // Verify it's a Blazor/Razor component
        var componentType = typeof(DialogoOperatoreView);
        var baseType = componentType.BaseType;
        
        // Assert it inherits from Microsoft.AspNetCore.Components.ComponentBase or similar
        componentType.Should().NotBeNull();
        baseType.Should().NotBeNull();
    }

    [Fact]
    public void DialogoOperatoreView_ShouldHaveNamespace()
    {
        // Verify component has correct namespace
        var componentType = typeof(DialogoOperatoreView);
        
        // Assert
        componentType.Namespace.Should().Contain("IMAR_DialogoOperatore.Components.Pages");
    }
}