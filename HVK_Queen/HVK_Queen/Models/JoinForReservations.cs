using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace HVK_Queen.Models {
    public class JoinForReservations {
        public virtual Reservation Reservation { get; set; }
        public virtual PetReservation PetReservation { get; set; }
        public virtual Pet Pet { get; set; }
    }
}