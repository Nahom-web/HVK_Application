using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace HVK_Queen.Models {
    public partial class PetVaccination {

        public DateTime ExpiryDate { get; set; }

        public int VaccinationId { get; set; }

        public int PetId { get; set; }

        public bool VaccinationChecked { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual Vaccination Vaccination { get; set; }

        [NotMapped]
        public string Name {
            get {
                return $"{ Vaccination.Name}";
            }
        }
    }
}
