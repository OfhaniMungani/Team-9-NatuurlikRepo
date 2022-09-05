using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.Models
{
    public class ConfirmationReminder
    {
        public int Id { get; set; }

        [Display(Name = "Days")]
        public string Days { get; set; }

        [Display(Name = "Value")]
        public int Value { get; set; }

        [Display(Name = "Active")]
        public string IsActive { get; set; }

    }
}
