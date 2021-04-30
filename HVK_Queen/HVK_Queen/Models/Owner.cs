using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static HVK_Queen.Models.CustomValidation;

namespace HVK_Queen.Models {
    public partial class Owner {

        private readonly HVK_QueenContext _context;

        public Owner(HVK_QueenContext context) {
            _context = context;
        }

        [NotMapped]
        private const string PhoneNumberRegex = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-.]?([0-9]{4})$";

        [NotMapped]
        private const string PostalCodeRegex = "^([A-Z][0-9]){3}$";

        public enum Provinces { ON, QC };

        public Owner() {
            Pet = new HashSet<Pet>();
        }

        public int OwnerId { get; set; }

        [Display(Name = "First Name:")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(25, ErrorMessage = "Please enter first name under {1} characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(25, ErrorMessage = "Please enter last name under {1} characters.")]
        public string LastName { get; set; }

        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "Please enter email under {1} characters.")]
        [CheckEmailAndPhoneFields("Email", "Phone", "CellPhone")]
        //Email phone custom validation
        public string Email { get; set; }

        [Display(Name = "Phone Number:")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(14, MinimumLength = 10, ErrorMessage = "Please provide 10 digit cell phone number.")]
        [RegularExpression(PhoneNumberRegex, ErrorMessage = "Please enter your phone number in the correct format (819)-274-3867")]
        [CheckEmailAndPhoneFields("Email", "Phone", "CellPhone")]
        public string Phone { get; set; }

        [Display(Name = "Cell Phone Number:")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(14, MinimumLength = 10, ErrorMessage = "Please provide 10 digit cell phone number.")]
        [RegularExpression(PhoneNumberRegex, ErrorMessage = "Please enter your cell number in the correct format (819)-274-3867")]
        [CheckEmailAndPhoneFields("Email", "Phone", "CellPhone")]
        public string CellPhone { get; set; }

        [Display(Name = "Street:")]
        [DataType(DataType.Text)]
        [MaxLength(40, ErrorMessage = "Please enter your street address under {1} characters")]
        public string Street { get; set; }

        [Display(Name = "City:")]
        [DataType(DataType.Text)]
        [MaxLength(25, ErrorMessage = "Please enter your city under {1} characters")]
        public string City { get; set; }

        [Display(Name = "Province:")]
        public string Province { get; set; }

        [Display(Name = "Postal Code:")]
        [DataType(DataType.Text)]
        [RegularExpression(PostalCodeRegex, ErrorMessage = "Please enter a valid postal code A1AA1A")]
        public string PostalCode { get; set; }

        public virtual Veterinarian Veterinarian { get; set; }

        [Display(Name = "First Name:")]
        [DataType(DataType.Text)]
        [MaxLength(25, ErrorMessage = "Please enter last name under 25 characters.")]
        [ValidateEmergencyContact("EmergencyContactFirstName", "EmergencyContactLastName", "EmergencyContactPhone")]
        public string EmergencyContactFirstName { get; set; }

        [Display(Name = "Last Name:")]
        [DataType(DataType.Text)]
        [MaxLength(25, ErrorMessage = "Please enter last name under 25 characters.")]
        [ValidateEmergencyContact("EmergencyContactFirstName", "EmergencyContactLastName", "EmergencyContactPhone")]
        public string EmergencyContactLastName { get; set; }

        [Display(Name = "Phone Number:")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(14, MinimumLength = 10, ErrorMessage = "Please provide 10 digit cell phone number.")]
        [RegularExpression(PhoneNumberRegex, ErrorMessage = "Please enter your phone number in the correct format (819)-274-3867")]
        [ValidateEmergencyContact("EmergencyContactFirstName", "EmergencyContactLastName", "EmergencyContactPhone")]
        public string EmergencyContactPhone { get; set; }

        public int? VeterinarianId { get; set; }

        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        [MaxLength(10, ErrorMessage = "Please Enter A Password Less Than 10 Characters")]
        public string Password { get; set; }

        public virtual ICollection<Pet> Pet { get; set; }

        public string stripPhoneNumber(string pNum) {
            if (pNum == null) {
                return null;
            }

            string str = "";

            for (int i = 0; i < pNum.Length; i++) {
                int x;
                bool result = int.TryParse($"{pNum[i]}", out x);
                if (result) {
                    str += pNum[i];
                }
            }
            return str;
        }

        public void UpdateOwnersInformation() {
            this.Phone = this.stripPhoneNumber(this.Phone);
            this.CellPhone = this.stripPhoneNumber(this.CellPhone);
            this.EmergencyContactPhone = this.stripPhoneNumber(this.EmergencyContactPhone);

            if (this.Province != "QC" || this.Province != "ON") {
                this.Province = null;
            }
        }

        public void UpdateOwnersVetInformation() {
            this.Veterinarian = new Veterinarian() {
                Name = this.Veterinarian.Name,
                Phone = this.stripPhoneNumber(this.Veterinarian.Phone),
                Street = this.Veterinarian.Street,
                City = this.Veterinarian.City,
                Province = this.Veterinarian.Province,
                PostalCode = this.Veterinarian.PostalCode
            };

            if (!(this.Veterinarian.Province == "QC" || this.Veterinarian.Province == "ON")) {
                this.Veterinarian.Province = null;
            }

        }

        public async System.Threading.Tasks.Task AddVetInformationAsync(List<Veterinarian> allVets) {

            var foundVet = allVets.Where(p => p.Phone == this.Veterinarian.Phone).FirstOrDefault();

            if (foundVet != null) {

                foundVet.Name = this.Veterinarian.Name;
                foundVet.Phone = this.Veterinarian.Phone;
                foundVet.Street = this.Veterinarian.Street;
                foundVet.City = this.Veterinarian.City;
                foundVet.Province = this.Veterinarian.Province;
                foundVet.PostalCode = this.Veterinarian.PostalCode;

                _context.Update(foundVet);
                await _context.SaveChangesAsync();

                this.Veterinarian = foundVet;
                this.VeterinarianId = foundVet.VeterinarianId;

            } else {

                // If the vet doesn't exisit it creates a new Veterinarian and adds it to the databse.

                _context.Add(this.Veterinarian);
                await _context.SaveChangesAsync();

                var updatedVet = _context.Veterinarian.Where(p => p.Phone == this.Veterinarian.Phone).FirstOrDefault();

                this.Veterinarian = updatedVet;
                this.VeterinarianId = updatedVet.VeterinarianId;
            }

        }


        public void DeleteOwnersInformation(List<PetReservation> petReservations) {

            for (int i = 0; i < petReservations.Count; i++) {

                if (petReservations[i].Reservation.ReservationDiscount != null) {
                    foreach (ReservationDiscount rm in petReservations[i].Reservation.ReservationDiscount) {
                        _context.ReservationDiscount.Remove(rm);
                    }
                }

                if (petReservations[i].Medications.Count >= 1) {
                    foreach (Medication med in petReservations[i].Medications) {
                        _context.Medication.Remove(med);
                    }
                }

                if (petReservations[i].PetReservationDiscount.Count >= 1) {
                    foreach (PetReservationDiscount prd in petReservations[i].PetReservationDiscount) {
                        _context.PetReservationDiscount.Remove(prd);
                    }
                }

                if (petReservations[i].PetReservationService.Count >= 1) {
                    foreach (PetReservationService prs in petReservations[i].PetReservationService) {
                        _context.PetReservationService.Remove(prs);
                    }
                }

                _context.PetReservation.Remove(petReservations[i]);

                _context.Reservation.Remove(petReservations[i].Reservation);

                if (petReservations[i].Pet.SpecialNotes != null) {
                    _context.SpecialNotes.Remove(petReservations[i].Pet.SpecialNotes);
                }

                if (petReservations[i].Pet.PetVaccination.Count >= 1) {
                    foreach (PetVaccination pv in petReservations[i].Pet.PetVaccination) {
                        _context.PetVaccination.Remove(pv);
                    }
                }

                _context.Pet.Remove(petReservations[i].Pet);

                _context.Owner.Remove(petReservations[i].Pet.Owner);

            }


            if (petReservations.Count == 0) {

                foreach (var p in this.Pet) {

                    _context.SpecialNotes.Remove(p.SpecialNotes);

                    if (p.PetVaccination.Count != 0) {
                        foreach (var pv in p.PetVaccination) {
                            _context.PetVaccination.Remove(pv);
                        }
                    }

                    _context.Pet.Remove(p);
                }

                _context.Owner.Remove(this);

            }
        }
    }
}