using IMS.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Persistence;

public class IMSDbContext : DbContext
{
    public IMSDbContext(DbContextOptions<IMSDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IMSDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
