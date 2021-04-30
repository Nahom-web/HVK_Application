using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HVK_Queen.Models.CustomValidation;

namespace HVK_Queen.Models {
    public partial class Pet {

        public enum Size { Small, Medium, Large }

        public enum Genders { Male, Female }

        public int PetId { get; set; }

        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(25, ErrorMessage = "The name must be no mare than 25 characters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        public bool Fixed { get; set; }

        [Display(Name = "Breed")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string Breed { get; set; }

        [Display(Name = "Birthday")]
        [DataType(DataType.Text)]
        [CheckBirthday()]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Birthday must be no mare than 10 characters")]
        public string Birthdate { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Display(Name = "Size")]
        [Required]
        public string DogSize { get; set; }

        [Required]
        public bool Climber { get; set; }

        [Required]
        public bool Barker { get; set; }

        public virtual Owner Owner { get; set; }

        public virtual SpecialNotes SpecialNotes { get; set; }

        public virtual ICollection<PetReservation> PetReservation { get; set; }

        public virtual ICollection<PetVaccination> PetVaccination { get; set; }

        public string getGender(string gender) {
            var newgender = "";
            if (gender == "Male")
                newgender = "M";
            else
                newgender = "F";
            return newgender;
        }
        public string getDogSize(string dogsize) {
            var newdogsize = "";
            if (dogsize == "Large")
                newdogsize = "L";
            else if (dogsize == "Medium")
                newdogsize = "M";
            else
                newdogsize = "S";
            return newdogsize;
        }

        public string getGenderFull(string gender) {
            var genderfull = "";
            if (gender == "M")
                genderfull = "Male";
            if (gender == "F")
                genderfull = "Female";
            return genderfull;
        }

        public string getDogSizefull(string dogsize) {
            var dogsizefull = "";
            if (dogsize == "L")
                dogsizefull = "Large";
            if (dogsize == "M")
                dogsizefull = "Medium";
            if (dogsize == "S")
                dogsizefull = "Small";
            return dogsizefull;
        }

        [NotMapped]
        public virtual ICollection<PetVaccination> AllPetVaccinations { get; set; }
    }
}
