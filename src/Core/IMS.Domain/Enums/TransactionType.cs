namespace IMS.Domain.Enums;

public enum TransactionType
{
    // Inbound Transactions (+1)
    Purchase = 1,    // Increases stock
    Return = 2,      // Increases stock
    TransferIn = 3,  // Increases stock at destination
    
    // Outbound Transactions (-1)
    Sale = -1,         // Decreases stock
    Consumption = -2,  // Decreases stock
    TransferOut = -3   // Decreases stock at source
}

public static class TransactionTypeExtensions
{
    public static int GetStockImpactMultiplier(this TransactionType type) => 
        (int)type > 0 ? 1 : -1;
}
