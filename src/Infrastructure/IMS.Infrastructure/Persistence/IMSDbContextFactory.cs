using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IMS.Infrastructure.Persistence;

public class IMSDbContextFactory : IDesignTimeDbContextFactory<IMSDbContext>
{
    public IMSDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/IMS.App")))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<IMSDbContext>();
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(IMSDbContext).Assembly.FullName));

        return new IMSDbContext(optionsBuilder.Options);
    }
}
