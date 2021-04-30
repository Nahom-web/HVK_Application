using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HVK_Queen.Models {
    public partial class Medication {

        public int MedicationId { get; set; }

        [Required]
        [Display(Name = "Name:")]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "This field must be less than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Dosage:")]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "This field must be less than 50 characters.")]
        public string Dosage { get; set; }

        [Required]
        [Display(Name = "End Date:")]
        //[DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Special Instructions:")]
        [MaxLength(50, ErrorMessage = "This field must be less than 50 characters.")]
        public string SpecialInstruct { get; set; }

        public int PetReservationId { get; set; }

        public virtual PetReservation PetReservation { get; set; }
    }
}
