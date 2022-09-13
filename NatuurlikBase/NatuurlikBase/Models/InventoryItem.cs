﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public class InventoryItem
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Inventory Item")]
        [MaxLength(50)]
        public string InventoryItemName { get; set; }

        [Required(ErrorMessage = "The Quantity On Hand Field is required.")]
        [RegularExpression("(^[0-9]*$)", ErrorMessage = "Quantity on Hand cannot be negative")]

        [Display(Name = "Quantity on Hand")]
        public int QuantityOnHand { get; set; }

        [Display(Name = "Threshold Value")]
        [Range(0, 10000)]
        public int ThresholdValue { get; set; }
        [Display(Name = "Inventory Type")]
        public int InventoryTypeId { get; set; }
        [ValidateNever]
        [ForeignKey("InventoryTypeId")]
        public InventoryType InventoryType { get; set; }

        public List<ProductInventory>? ProductInventories { get; set; }

    }
}