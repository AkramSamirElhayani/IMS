using System.Linq.Expressions;
using IMS.Application.Common.Interfaces;
using IMS.Domain.Aggregates;
using IMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Persistence.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly IMSDbContext _context;

    public ItemRepository(IMSDbContext context)
    {
        _context = context;
    }

    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Item?> GetBySkuAsync(SKU sku, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .FirstOrDefaultAsync(x => x.SKU == sku, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Items.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(SKU sku, CancellationToken cancellationToken = default)
    {
        return await _context.Items.AnyAsync(x => x.SKU == sku, cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetItemsByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .Where(x => x.Type.ToString() == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetActiveItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetItemsByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .Where(x => x.StorageLocations.Any(sl => sl == location))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetPerishableItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .Where(x => x.IsPerishable )
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetItemsBelowReorderPointAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .Where(x => x.StockLevel.Current <= x.StockLevel.Minimum)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> FindAsync(Expression<Func<Item, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StorageLocations)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Item entity, CancellationToken cancellationToken = default)
    {
        await _context.Items.AddAsync(entity, cancellationToken);

    }

    public async Task AddRangeAsync(IEnumerable<Item> entities, CancellationToken cancellationToken = default)
    {
        await _context.Items.AddRangeAsync(entities, cancellationToken);

    }

    public Task UpdateAsync(Item entity, CancellationToken cancellationToken = default)
    {
        _context.Items.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Item> entities, CancellationToken cancellationToken = default)
    {
        _context.Items.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Item entity, CancellationToken cancellationToken = default)
    {
        _context.Items.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<Item> entities, CancellationToken cancellationToken = default)
    {
        _context.Items.RemoveRange(entities);
        return Task.CompletedTask;
    }
}
