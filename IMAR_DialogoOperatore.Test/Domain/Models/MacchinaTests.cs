using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Test.Domain.Models;

public class MacchinaTests
{
    [Fact]
    public void Macchina_DefaultConstructor_ShouldInitializeProperties()
    {
        var macchina = new Macchina();

        macchina.Should().NotBeNull();
        macchina.CodiceJMes.Should().Be(0);
    }

    [Fact]
    public void Macchina_Properties_ShouldBeSettableAndGettable()
    {
        var macchina = new Macchina
        {
            CentroDiLavoro = "CL001",
            CodiceMacchina = "M001",
            CodiceJMes = 12345
        };

        macchina.CentroDiLavoro.Should().Be("CL001");
        macchina.CodiceMacchina.Should().Be("M001");
        macchina.CodiceJMes.Should().Be(12345);
    }

    [Theory]
    [InlineData("CL001", "M001", 1)]
    [InlineData("CL002", "M002", 2)]
    [InlineData("CL999", "M999", 999)]
    public void Macchina_ShouldAcceptVariousValues(string centroDiLavoro, string codiceMacchina, int codiceJMes)
    {
        var macchina = new Macchina
        {
            CentroDiLavoro = centroDiLavoro,
            CodiceMacchina = codiceMacchina,
            CodiceJMes = codiceJMes
        };

        macchina.CentroDiLavoro.Should().Be(centroDiLavoro);
        macchina.CodiceMacchina.Should().Be(codiceMacchina);
        macchina.CodiceJMes.Should().Be(codiceJMes);
    }

    [Fact]
    public void Macchina_StringProperties_ShouldAcceptNullValues()
    {
        var macchina = new Macchina
        {
            CentroDiLavoro = null!,
            CodiceMacchina = null!
        };

        macchina.CentroDiLavoro.Should().BeNull();
        macchina.CodiceMacchina.Should().BeNull();
    }
}