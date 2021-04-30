using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HVK_Queen.Models {
    public class Login {

        [Display(Name = "Email Or Phone:")]
        [Required]
        public string EmailOrPhone { get; set; }

        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public bool CheckIfClerk(String email) {
            if (email.Equals(null)) {
                return false;
            } else {
                string test = email[^7..];
                if (email[^7..].Equals("@hvk.ca")) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
