using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IMS.Domain.Aggregates;
using IMS.Domain.ValueObjects;

namespace IMS.Application.Common.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item?> GetBySkuAsync(SKU sku, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(SKU sku, CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetItemsByTypeAsync(string type, CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetActiveItemsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetItemsByLocationAsync(string location, CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetPerishableItemsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetItemsBelowReorderPointAsync(CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Item>> SearchItemsAsync(
            string? searchTerm,
            int? minQuantity = null,
            int? maxQuantity = null,
            string? sortBy = null,
            bool isAscending = true,
            CancellationToken cancellationToken = default);

        Task<int> GetTotalItemCountAsync(
            string? searchTerm = null,
            int? minQuantity = null,
            int? maxQuantity = null,
            CancellationToken cancellationToken = default);
    }
}
