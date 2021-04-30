using System;
using System.Collections.Generic;

namespace HVK_Queen.Models {
    public partial class KennelLog {

        public DateTime LogDate { get; set; }

        public int SequenceNumber { get; set; }

        public string Notes { get; set; }

        public int PetReservationId { get; set; }

        public virtual PetReservation PetReservation { get; set; }
    }
}
