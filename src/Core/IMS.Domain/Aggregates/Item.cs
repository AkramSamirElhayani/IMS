using System;
using System.Collections.Generic;
using IMS.Domain.Common;
using IMS.Domain.Enums;
using IMS.Domain.Events;
using IMS.Domain.ValueObjects;

namespace IMS.Domain.Aggregates
{
    public sealed class Item : Entity
    {
        public Guid Id { get; private set; }
        public SKU SKU { get; private set; }
        public string Name { get; private set; }
        public ItemType Type { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsPerishable { get; private set; }
        public QualityStatus QualityStatus { get; private set; }
        public StockLevel StockLevel { get; private set; }
        public List<string> StorageLocations { get; private set; }

        private Item(
            SKU sku,
            string name,
            ItemType type,
            bool isPerishable,
            StockLevel stockLevel)
        {
            Id = Guid.NewGuid();
            SKU = sku ?? throw new ArgumentNullException(nameof(sku));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            IsActive = true;
            IsPerishable = isPerishable;
            QualityStatus = QualityStatus.Good;
            StockLevel = stockLevel ?? throw new ArgumentNullException(nameof(stockLevel));
            StorageLocations = new List<string>();

            AddDomainEvent(new ItemCreatedEvent(Id, sku.Value, type));
        }

        public static Item Create(
            SKU sku,
            string name,
            ItemType type,
            bool isPerishable,
            StockLevel stockLevel)
        {
            return new Item(sku, name, type, isPerishable, stockLevel);
        }

        public void UpdateQualityStatus(QualityStatus newStatus)
        {
            var oldStatus = QualityStatus;
            QualityStatus = newStatus;
            
            AddDomainEvent(new QualityStatusChangedEvent(Id, oldStatus, newStatus));
        }

        public void UpdateStockLevel(int newQuantity, Guid? transactionId = null)
        {
            var oldQuantity = StockLevel.Current;
            StockLevel = StockLevel.Create(newQuantity, StockLevel.Minimum, StockLevel.Maximum, StockLevel.Critical);
            
            AddDomainEvent(new StockLevelChangedEvent(Id, oldQuantity, newQuantity, transactionId));

            if (newQuantity <= StockLevel.Critical)
            {
                AddDomainEvent(new CriticalStockLevelReachedEvent(Id, newQuantity, StockLevel.Critical));
            }
        }



        public void AddStorageLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be empty", nameof(location));

            if (!StorageLocations.Contains(location))
                StorageLocations.Add(location);
        }

        public void RemoveStorageLocation(string location)
        {
            StorageLocations.Remove(location);
        }

        public void Deactivate()
        {
            IsActive = false;
            AddDomainEvent(new ItemDeactivatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            AddDomainEvent(new ItemActivatedEvent(Id));
        }

        public void UpdateBasicProperties(
            string name,
            ItemType type,
            bool isPerishable)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            IsPerishable = isPerishable;
            
            AddDomainEvent(new ItemUpdatedEvent(Id));
        }

        public bool CanWithdraw(int quantity)
        {
            return IsActive 
                && QualityStatus != QualityStatus.Quarantined 
                && StockLevel.Current >= quantity;
        }
    }
}
