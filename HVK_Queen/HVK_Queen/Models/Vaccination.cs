using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HVK_Queen.Models {
    public partial class Vaccination {

        public Vaccination() {
            PetVaccination = new HashSet<PetVaccination>();
        }

        public int VaccinationId { get; set; }

        [Display(Name = "Vaccination Name:")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<PetVaccination> PetVaccination { get; set; }
    }
}
