using IMS.Domain.Aggregates;
using IMS.Domain.ValueObjects;

namespace IMS.Application.Common.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<Item?> GetBySkuAsync(SKU sku, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SKU sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetItemsByTypeAsync(string type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetActiveItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetItemsByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetPerishableItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetItemsBelowReorderPointAsync(CancellationToken cancellationToken = default);
}
