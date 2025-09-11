using FluentAssertions;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Test.Helpers;
using System.Diagnostics;

namespace IMAR_DialogoOperatore.Test.Performance;

public class PerformanceTests
{
    [Fact]
    public void TestDataBuilder_CreateMultipleAttivita_ShouldBeEfficient()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        const int count = 1000;

        // Act
        var attivita = TestDataBuilder.CreateAttivitaList(count);
        stopwatch.Stop();

        // Assert
        attivita.Should().HaveCount(count);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should complete in less than 100ms
    }

    [Fact]
    public void Operatore_AddManyAttivita_ShouldPerformWell()
    {
        // Arrange
        var operatore = TestDataBuilder.CreateDefaultOperatore();
        operatore.AttivitaAperte = new List<Attivita>();
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            operatore.AttivitaAperte.Add(TestDataBuilder.CreateDefaultAttivita($"B{i:D6}"));
        }
        stopwatch.Stop();

        // Assert
        operatore.AttivitaAperte.Should().HaveCount(1000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(50); // Should be very fast
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Attivita_BulkOperations_ShouldScaleLinearly(int count)
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var attivita = new List<Attivita>();
        for (int i = 0; i < count; i++)
        {
            attivita.Add(new Attivita
            {
                Bolla = $"B{i:D6}",
                Odp = $"ODP{i:D3}",
                QuantitaOrdine = i * 10,
                QuantitaProdotta = i * 5,
                QuantitaScartata = i,
                InizioAttivita = DateTime.Now.AddHours(-i)
            });
        }
        stopwatch.Stop();

        // Assert
        attivita.Should().HaveCount(count);
        
        // Performance should scale reasonably (allowing for variance in test environment)
        var maxAllowedMs = count / 10 + 10; // Very generous performance target
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxAllowedMs);
    }

    [Fact]
    public void Macchina_CreationInLoop_ShouldBeEfficient()
    {
        // Arrange
        var machines = new List<Macchina>();
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 500; i++)
        {
            machines.Add(new Macchina
            {
                CodiceMacchina = $"M{i:D4}",
                CentroDiLavoro = $"CL{i % 10:D2}",
                CodiceJMes = i * 1000
            });
        }
        stopwatch.Stop();

        // Assert
        machines.Should().HaveCount(500);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(25);
    }

    [Fact]
    public void Operatore_WithManyAttivitaQueries_ShouldPerformWell()
    {
        // Arrange
        var operatore = TestDataBuilder.CreateOperatoreWithAttivita(100);
        var stopwatch = Stopwatch.StartNew();

        // Act
        var inLavoroCount = operatore.AttivitaAperte.Count(a => a.Causale == "IN_LAVORO");
        var completedCount = operatore.AttivitaAperte.Count(a => a.FineAttivita != null);
        var totalQuantity = operatore.AttivitaAperte.Sum(a => a.QuantitaOrdine);
        
        stopwatch.Stop();

        // Assert
        operatore.AttivitaAperte.Should().HaveCount(100);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10); // LINQ operations should be very fast
    }

    [Fact]
    public void Memory_LargeObjectCreation_ShouldNotCauseIssues()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);

        // Act
        var largeList = new List<Attivita>();
        for (int i = 0; i < 10000; i++)
        {
            largeList.Add(TestDataBuilder.CreateDefaultAttivita($"LARGE_{i}"));
        }

        var afterCreationMemory = GC.GetTotalMemory(false);
        largeList.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var afterCleanupMemory = GC.GetTotalMemory(true);

        // Assert
        largeList.Should().BeEmpty();
        afterCreationMemory.Should().BeGreaterThan(initialMemory);
        afterCleanupMemory.Should().BeLessThan(afterCreationMemory); // Memory should be reclaimed
    }
}