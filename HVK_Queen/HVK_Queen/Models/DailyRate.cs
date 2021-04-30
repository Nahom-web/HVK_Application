using System;
using System.Collections.Generic;

namespace HVK_Queen.Models {
    public partial class DailyRate {
        public int DailyRateId { get; set; }

        public decimal Rate { get; set; }

        public string DogSize { get; set; }

        public int ServiceId { get; set; }

        public virtual Service Service { get; set; }
    }
}
