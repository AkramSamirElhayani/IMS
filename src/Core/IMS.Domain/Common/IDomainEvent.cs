using MediatR;

namespace IMS.Domain.Common;

/// <summary>
/// Represents a domain event that can be published and handled within the application.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime DateOccurred { get; }
}
