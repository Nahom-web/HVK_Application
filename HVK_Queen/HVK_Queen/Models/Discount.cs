using System;
using System.Collections.Generic;


namespace HVK_Queen.Models {
    public partial class Discount {

        public Discount() {
            PetReservationDiscount = new HashSet<PetReservationDiscount>();
            ReservationDiscount = new HashSet<ReservationDiscount>();
        }

        public int DiscountId { get; set; }

        public string Desciption { get; set; }

        public decimal Percentage { get; set; }

        public string Type { get; set; }

        public virtual ICollection<PetReservationDiscount> PetReservationDiscount { get; set; }

        public virtual ICollection<ReservationDiscount> ReservationDiscount { get; set; }
    }
}
