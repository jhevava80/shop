using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(4)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }
    }
}
