using System;
using System.Collections.Generic;


namespace HVK_Queen.Models {
    public partial class Run {

        public Run() {
            PetReservation = new HashSet<PetReservation>();
        }

        public int RunId { get; set; }

        public string Size { get; set; }

        public int Covered { get; set; }

        public string Location { get; set; }

        public decimal? Status { get; set; }

        public virtual ICollection<PetReservation> PetReservation { get; set; }
    }
}
