using System;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;

namespace IMS.Application.Features.Items.Common.Responses
{
    public class GetItemResponse
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ItemType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsPerishable { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public StockLevel StockLevel { get; set; }
        public List<string> StorageLocations { get; set; } = new();
    }
}
