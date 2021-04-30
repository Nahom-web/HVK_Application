using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HVK_Queen.Models {
    public partial class PetReservation {
        public PetReservation() {
            KennelLog = new HashSet<KennelLog>();
            Medications = new List<Medication>();
            PetReservationDiscount = new HashSet<PetReservationDiscount>();
            PetReservationService = new HashSet<PetReservationService>();
        }

        public int PetReservationId { get; set; }

        public int PetId { get; set; }

        public int ReservationId { get; set; }

        public int? RunId { get; set; }

        // Service Id: 2
        [NotMapped]
        public bool Walk { get; set; }

        // Service Id: 5
        [NotMapped]
        public bool Playtime { get; set; }

        [NotMapped]
        public bool InReservation { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual Reservation Reservation { get; set; }

        public virtual Run Run { get; set; }

        public virtual ICollection<KennelLog> KennelLog { get; set; }

        public virtual List<Medication> Medications { get; set; }

        public virtual ICollection<PetReservationDiscount> PetReservationDiscount { get; set; }

        public virtual ICollection<PetReservationService> PetReservationService { get; set; }

        public void SetServicesSelected() {
            foreach (var prs in PetReservationService) {
                if (prs.Service.ServiceId == 2) {
                    Walk = true;
                }
                if (prs.Service.ServiceId == 5) {
                    Playtime = true;
                }
            }
        }

        public void addMedication() {
            Medication newMedication = new Medication();
            this.Medications.Add(newMedication);
        }

    }
}
