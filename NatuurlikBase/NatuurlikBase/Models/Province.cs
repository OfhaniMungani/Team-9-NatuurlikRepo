﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public class Province
    {
        public Province()
        {
            this.City = new HashSet<City>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Province Name")]
        [RegularExpression(@"^[a-zA-Z]+[ ]?([a-zA-Z]+[ ]?)*$",
         ErrorMessage = "Invalid Province Name: Two consecutive white spaces and digits are not allowed.")]
        public string ProvinceName { get; set; }
        [Required]
        public int CountryId { get; set; }
        [ValidateNever]
        [ForeignKey("CountryId")]
        public Country Country { get; set; }
        public ICollection<City> City { get; set; }

    }
}
