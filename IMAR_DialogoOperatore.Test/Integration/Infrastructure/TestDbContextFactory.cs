using IMAR_DialogoOperatore.Infrastructure.Imar_Connect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IMAR_DialogoOperatore.Test.Integration.Infrastructure;

public class TestDbContextFactory
{
    public static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();
        
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ImarConnect"] = "Data Source=:memory:"
            })
            .Build();
            
        services.AddSingleton<IConfiguration>(configuration);

        // Add DbContext with SQLite in-memory for testing
        services.AddDbContext<ImarConnectContext>(options =>
            options.UseSqlite("Data Source=:memory:"));

        return services.BuildServiceProvider();
    }

    public static ImarConnectContext CreateImarConnectContext()
    {
        var options = new DbContextOptionsBuilder<ImarConnectContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ImarConnect"] = "Data Source=:memory:"
            })
            .Build();

        var context = new ImarConnectContext(options, configuration);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

}