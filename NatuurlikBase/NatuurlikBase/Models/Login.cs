﻿using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.Models
{
    public class Login

    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
