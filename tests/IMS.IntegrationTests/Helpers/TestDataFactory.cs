using System;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;

namespace IMS.IntegrationTests.Helpers
{
    public static class TestDataFactory
    {
        public static class Transactions
        {
            public static Transaction CreatePurchase(
                Guid itemId,
                int quantity,
                string location = "Warehouse A",
                bool isCompleted = true,
                DateTimeOffset? transactionDate = null)
            {
                var transaction = Transaction.CreateInbound(
                    itemId,
                    quantity,
                    location,
                    TransactionType.Purchase,
                    BatchInformation.Create(
                        "BATCH-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddYears(1)),
                    transactionDate ?? DateTimeOffset.UtcNow);

                if (isCompleted)
                {
                    transaction.Complete();
                }

                return transaction;
            }

            public static Transaction CreateSale(
                Guid itemId,
                int quantity,
                string location = "Warehouse A",
                bool isCompleted = true,
                DateTimeOffset? transactionDate = null)
            {
                var transaction = Transaction.CreateOutbound(
                    itemId,
                    quantity,
                    location,
                    TransactionType.Sale,
                    BatchInformation.Create(
                        "BATCH-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddYears(1)),
                    transactionDate ?? DateTimeOffset.UtcNow);

                if (isCompleted)
                {
                    transaction.Complete();
                }

                return transaction;
            }

            public static Transaction CreateReturn(
                Guid itemId,
                int quantity,
                string location = "Returns Department",
                bool isCompleted = true,
                DateTimeOffset? transactionDate = null)
            {
                var transaction = Transaction.CreateInbound(
                    itemId,
                    quantity,
                    location,
                    TransactionType.Return,
                    BatchInformation.Create(
                        "BATCH-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddYears(1)),
                    transactionDate ?? DateTimeOffset.UtcNow);

                if (isCompleted)
                {
                    transaction.Complete();
                }

                return transaction;
            }

            public static Transaction CreateTransferIn(
                Guid itemId,
                int quantity,
                string destinationLocation = "Warehouse B",
                bool isCompleted = true,
                DateTimeOffset? transactionDate = null)
            {
                var transaction = Transaction.CreateInbound(
                    itemId,
                    quantity,
                    destinationLocation,
                    TransactionType.TransferIn,
                    BatchInformation.Create(
                        "BATCH-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddYears(1)),
                    transactionDate ?? DateTimeOffset.UtcNow);

                if (isCompleted)
                {
                    transaction.Complete();
                }

                return transaction;
            }

            public static Transaction CreateTransferOut(
                Guid itemId,
                int quantity,
                string sourceLocation = "Warehouse A",
                bool isCompleted = true,
                DateTimeOffset? transactionDate = null)
            {
                var transaction = Transaction.CreateOutbound(
                    itemId,
                    quantity,
                    sourceLocation,
                    TransactionType.TransferOut,
                    BatchInformation.Create(
                        "BATCH-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddYears(1)),
                    transactionDate ?? DateTimeOffset.UtcNow);

                if (isCompleted)
                {
                    transaction.Complete();
                }

                return transaction;
            }
        }

        public static class Items
        {
            public static Item Create(
                string name = null,
                string sku = null,
                bool isPerishable = false)
            {
                name ??= "Test Item " + Guid.NewGuid().ToString("N").Substring(0, 8);
                sku ??= "SKU-" + Guid.NewGuid().ToString("N").Substring(0, 8);

                var skuObj = SKU.Create(sku);
                var stockLevel = StockLevel.Create(0, 10, 100, 20); // Default stock levels for testing

                return Item.Create(
                    skuObj,
                    name,
                    ItemType.FinishedGood, // Default type for testing
                    isPerishable,
                    stockLevel);
            }
        }
    }
}
