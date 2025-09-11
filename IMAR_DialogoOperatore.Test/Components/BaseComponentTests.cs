using FluentAssertions;
using IMAR_DialogoOperatore.Test.Helpers;

namespace IMAR_DialogoOperatore.Test.Components;

public class BaseComponentTests
{
    [Fact]
    public void BlazorTestHelper_ShouldExist()
    {
        // Verify the helper class exists
        var helperType = typeof(BlazorTestHelper);
        
        // Assert
        helperType.Should().NotBeNull();
        helperType.IsPublic.Should().BeTrue();
        helperType.IsAbstract.Should().BeTrue();
        helperType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void BlazorTestHelper_ShouldHaveConfigureMethod()
    {
        // Verify the configure method exists
        var method = typeof(BlazorTestHelper).GetMethod("ConfigureBlazorServices");
        
        // Assert
        method.Should().NotBeNull();
        method!.IsPublic.Should().BeTrue();
        method.IsStatic.Should().BeTrue();
    }
}