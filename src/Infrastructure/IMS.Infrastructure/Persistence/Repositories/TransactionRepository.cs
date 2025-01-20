using System.Linq.Expressions;
using IMS.Application.Common.Interfaces;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace IMS.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IMSDbContext _context;
    private readonly string _connectionString;

    public TransactionRepository(IMSDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => x.ItemId == productId)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => x.TransactionDate >= start && x.TransactionDate <= end)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(x => !x.IsCompleted)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> FindAsync(Expression<Func<Transaction, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(predicate)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> entities, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddRangeAsync(entities, cancellationToken);
    }

    public Task UpdateAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Transaction> entities, CancellationToken cancellationToken = default)
    {
        _context.Transactions.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<Transaction> entities, CancellationToken cancellationToken = default)
    {
        _context.Transactions.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<int> CalculateTotalStockForItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT COALESCE(SUM(t.Quantity * 
                CASE WHEN CAST(t.Type as int) > 0 THEN 1 ELSE -1 END
            ), 0)
            FROM Transactions t
            WHERE t.ItemId = @ItemId 
            AND t.Status = TRUE";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleAsync<int>(
            sql,
            new { 
                ItemId = itemId
            });
    }
}
