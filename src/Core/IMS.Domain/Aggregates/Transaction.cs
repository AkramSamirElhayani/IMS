using System;
using IMS.Domain.Common;
using IMS.Domain.Enums;
using IMS.Domain.Events;
using IMS.Domain.Events.Transactions;
using IMS.Domain.ValueObjects;

namespace IMS.Domain.Aggregates
{
    public sealed class Transaction : Entity
    {
        public Guid Id { get; private set; }
        public TransactionReference Reference { get; private set; }
        public TransactionType Type { get; private set; }
        public Guid ItemId { get; private set; }
        public int Quantity { get; private set; }
        public string SourceLocation { get; private set; }
        public string DestinationLocation { get; private set; }
        public BatchInformation BatchInfo { get; private set; }
        public DateTimeOffset TransactionDate { get; private set; }
        public bool IsCompleted { get; private set; }
        private Transaction(
            TransactionReference reference,
            TransactionType type,
            Guid itemId,
            int quantity,
            string sourceLocation,
            string destinationLocation,
            BatchInformation batchInfo = null,
            DateTimeOffset? transactionDate = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Id = Guid.NewGuid();
            Reference = reference ?? throw new ArgumentNullException(nameof(reference));
            Type = type;
            ItemId = itemId;
            Quantity = quantity;
            SourceLocation = sourceLocation;
            DestinationLocation = destinationLocation;
            BatchInfo = batchInfo;
            TransactionDate = transactionDate ?? DateTimeOffset.UtcNow;
            IsCompleted = false;
            AddDomainEvent(new TransactionCreatedEvent(Id, ItemId, Type, Quantity));
        }

        public static Transaction CreateInbound(
            Guid itemId,
            int quantity,
            string destinationLocation,
            TransactionType type,
            BatchInformation batchInfo = null,
            DateTimeOffset? transactionDate = null)
        {
            if (!IsInboundTransaction(type))
                throw new ArgumentException("Invalid transaction type for inbound transaction", nameof(type));

            var reference = TransactionReference.Create(DateTime.UtcNow, "IN");

            return new Transaction(
                reference,
                type,
                itemId,
                quantity,
                null,
                destinationLocation,
                batchInfo,
                transactionDate);
        }

        public static Transaction CreateOutbound(
            Guid itemId,
            int quantity,
            string sourceLocation,
            TransactionType type,
            BatchInformation? batchInfo = null,
            DateTimeOffset? transactionDate = null)
        {
            if (!IsOutboundTransaction(type))
                throw new ArgumentException("Invalid transaction type for outbound transaction", nameof(type));

            var reference = TransactionReference.Create(DateTime.UtcNow, "OUT");

            return new Transaction(
                reference,
                type,
                itemId,
                quantity,
                sourceLocation,
                null,
                batchInfo,
                transactionDate);
        }

        public static Transaction CreateInternal(
            Guid itemId,
            int quantity,
            string sourceLocation,
            string destinationLocation,
            TransactionType type,
            BatchInformation batchInfo = null,
            DateTimeOffset? transactionDate = null)
        {
            if (!IsInternalTransaction(type))
                throw new ArgumentException("Invalid transaction type for internal transaction", nameof(type));

            var reference = TransactionReference.Create(DateTime.UtcNow, "INT");

            return new Transaction(
                reference,
                type,
                itemId,
                quantity,
                sourceLocation,
                destinationLocation,
                batchInfo,
                transactionDate);
        }

        private static bool IsInboundTransaction(TransactionType type)
        {
            return type == TransactionType.Purchase ||
                   type == TransactionType.Return ||
                   type == TransactionType.TransferIn;
        }

        private static bool IsOutboundTransaction(TransactionType type)
        {
            return type == TransactionType.Sale ||
                   type == TransactionType.Consumption ||
                   type == TransactionType.TransferOut;
        }

        private static bool IsInternalTransaction(TransactionType type)
        {
            return type == TransactionType.QualityStatusChange ||
                   type == TransactionType.LocationTransfer;
        }

        // Helper method to determine if this transaction affects stock levels
        public bool AffectsStock()
        {
            return Type != TransactionType.QualityStatusChange;
        }

        // Helper method to determine if this is a stock increase
        public bool IsStockIncrease()
        {
            return Type == TransactionType.Purchase ||
                   Type == TransactionType.Return ||
                   Type == TransactionType.TransferIn;
        }

        public void Complete()
        {
            IsCompleted = true;
            AddDomainEvent(new TransactionCompletedEvent(
                Id,
                ItemId,
                Type,
                Quantity,
                SourceLocation,
                DestinationLocation));
        }
    }
}
