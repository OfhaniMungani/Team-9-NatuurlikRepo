using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public class SupplierInventory
    {
        [Key]
        public int SupplierOrderId { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public int InventoryItemId { get; set; }
        public InventoryItem? Inventory { get; set; }
        public int InventoryItemQuantity { get; set; }
    }
}
