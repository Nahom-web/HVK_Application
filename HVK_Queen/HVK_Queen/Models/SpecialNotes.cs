using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HVK_Queen.Models {
    public partial class SpecialNotes {

        [Display(Name = "Special Notes")]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int PetId { get; set; }

        public virtual Pet Pet { get; set; }
    }
}