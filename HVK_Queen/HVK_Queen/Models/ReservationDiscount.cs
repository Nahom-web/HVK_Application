using System;
using System.Collections.Generic;


namespace HVK_Queen.Models {
    public partial class ReservationDiscount {

        public int DiscountId { get; set; }

        public int ReservationId { get; set; }

        public virtual Discount Discount { get; set; }

        public virtual Reservation Reservation { get; set; }
    }
}
