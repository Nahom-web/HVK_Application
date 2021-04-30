using System;
using System.Collections.Generic;


namespace HVK_Queen.Models {
    public partial class Service {

        public Service() {
            DailyRate = new HashSet<DailyRate>();
            PetReservationService = new HashSet<PetReservationService>();
        }

        public int ServiceId { get; set; }

        public string ServiceDescription { get; set; }

        public virtual ICollection<DailyRate> DailyRate { get; set; }

        public virtual ICollection<PetReservationService> PetReservationService { get; set; }
    }
}
