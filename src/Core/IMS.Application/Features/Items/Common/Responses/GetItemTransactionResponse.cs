using System;

namespace IMS.Application.Features.Items.Common.Responses
{
    public class GetItemTransactionResponse
    {
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }
}
