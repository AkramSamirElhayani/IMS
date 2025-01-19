namespace IMS.Domain.Enums
{
    public enum TransactionType
    {
        // Inbound Transactions
        Purchase,
        Return,
        TransferIn,
        
        // Outbound Transactions
        Sale,
        Consumption,
        TransferOut,
        
        // Internal Transactions
        QualityStatusChange,
        LocationTransfer
    }
}
