using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static HVK_Queen.Models.CustomValidation;

namespace HVK_Queen.Models {

    public partial class Veterinarian {

        private const string PhoneNumberRegex = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-.]?([0-9]{4})$";

        private const string PostalCodeRegex = "^([A-Z][0-9]){3}$";

        public enum Provinces { ON, QC };

        public Veterinarian() {
            Owner = new HashSet<Owner>();
        }

        public int VeterinarianId { get; set; }

        [Display(Name = "Name:")]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "Please enter your vet's name under 50 characters")]
        [ValidateVeterinarianInformation("Name", "Phone", "Street", "City", "PostalCode")]
        public string Name { get; set; }

        [Display(Name = "Phone:")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(14, MinimumLength = 10, ErrorMessage = "Please enter your phone number between {2} - {1} characters")]
        [RegularExpression(PhoneNumberRegex, ErrorMessage = "Please enter your phone number in the correct format (111)-111-1111")]
        [ValidateVeterinarianInformation("Name", "Phone", "Street", "City", "PostalCode")]
        public string Phone { get; set; }

        [Display(Name = "Street:")]
        [DataType(DataType.Text)]
        [MaxLength(40, ErrorMessage = "Please enter your street address under {1} characters")]
        public string Street { get; set; }

        [Display(Name = "City:")]
        [DataType(DataType.Text)]
        [MaxLength(25, ErrorMessage = "Please enter your city under {1} characters")]
        public string City { get; set; }

        [Display(Name = "Province:")]
        public string Province { get; set; }

        [Display(Name = "Postal Code:")]
        [DataType(DataType.Text)]
        [RegularExpression(PostalCodeRegex, ErrorMessage = "Please enter a valid postal code A1AA1A")]
        public string PostalCode { get; set; }

        public virtual ICollection<Owner> Owner { get; set; }
    }
}
