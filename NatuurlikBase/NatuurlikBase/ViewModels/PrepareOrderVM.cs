﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using NatuurlikBase.Models;
using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.ViewModels
{
    public class PrepareOrderVM
    {
        public Order Order { get; set; }
        [ValidateNever]
        public IEnumerable<PackageOrderProduct> PackageOrderProduct { get; set; }
    }
}
