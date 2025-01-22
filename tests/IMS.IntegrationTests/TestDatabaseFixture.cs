using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;
using Xunit;
using IMS.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.IntegrationTests
{
    public class TestDatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _container;
        private IServiceProvider _serviceProvider;
        public string ConnectionString { get; private set; }

        public TestDatabaseFixture()
        {
            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:latest")
                .WithHostname($"integration_tests_db_{Guid.NewGuid()}")
                .WithPassword("Your_password123")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();

            // Setup service provider with DbContext
            var services = new ServiceCollection();
            
            services.AddDbContext<IMSDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            _serviceProvider = services.BuildServiceProvider();

            // Run EF Core migrations
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IMSDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            await _container.DisposeAsync();
        }

        public IMSDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<IMSDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new IMSDbContext(options);
        }
    }
}
