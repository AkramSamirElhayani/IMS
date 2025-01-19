using IMS.Application.Common.Interfaces;
using IMS.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace IMS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMSDbContext _context;
    private readonly IMediator _mediator;
    private IDbContextTransaction? _currentTransaction;
    private readonly List<IDomainEvent> _collectedDomainEvents;

    public UnitOfWork(IMSDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
        _collectedDomainEvents = new List<IDomainEvent>();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get all entities that have domain events
        var entities = _context.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // Collect domain events
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // If we're in a transaction, collect events for later publishing
        if (_currentTransaction != null)
        {
            _collectedDomainEvents.AddRange(domainEvents);
        }

        // Save changes
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Only clear events after successful save
        entities.ForEach(e => e.ClearDomainEvents());

        // If we're not in a transaction, publish events immediately
        if (_currentTransaction == null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }

        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _collectedDomainEvents.Clear(); // Clear any previously collected events
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress");
            }

            await _currentTransaction.CommitAsync(cancellationToken);

            // After successful commit, publish all collected domain events
            foreach (var domainEvent in _collectedDomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
                _collectedDomainEvents.Clear(); // Clear events after transaction completes
            }
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
            _collectedDomainEvents.Clear(); // Clear events on rollback
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _collectedDomainEvents.Clear();
    }
}
