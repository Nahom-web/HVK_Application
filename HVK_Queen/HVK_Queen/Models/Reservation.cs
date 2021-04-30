using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static HVK_Queen.Models.CustomValidation;

namespace HVK_Queen.Models {
    public partial class Reservation {
        //Failed to get the ID from the session
        const int OWNERID = 2;

        [NotMapped]
        private readonly HVK_QueenContext _context;

        public Reservation(HVK_QueenContext context) {
            _context = context;
        }

        [NotMapped]
        private const double GST = 0.05;

        [NotMapped]
        private const double QST = 0.09975;

        public enum Services { DailyWalk, DailyPlaytime, Medication }

        public int ReservationId { get; set; }

        [Display(Name = "Start Date:")]
        [DataType(DataType.Date)]
        [Required]
        [CheckStartDate()]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date:")]
        [DataType(DataType.Date)]
        [Required]
        [CheckEndDate("StartDate")]
        public DateTime EndDate { get; set; }

        public decimal Status { get; set; }

        public virtual ICollection<PetReservation> PetReservation { get; set; }

        public virtual ICollection<ReservationDiscount> ReservationDiscount { get; set; }

        public Reservation() {

            _context = new HVK_QueenContext();

            PetReservation = SetPetRes(OWNERID);
        }

        public ICollection<PetReservation> SetPetRes(int ownerId) {
            var pets = _context.Pet.Where(x => x.OwnerId == ownerId).ToList();
            var newPetRes = new Collection<PetReservation>();

            foreach (var x in pets) {
                newPetRes.Add(new PetReservation() { PetId = x.PetId });  
            }

            return newPetRes;
        }

        public void SetMissingPetRes(int ownerId) {
            var pets = _context.Pet.Where(x => x.OwnerId == ownerId).ToList();
            bool inRes;
            foreach (var p in pets) {
                inRes = false;
                foreach (var r in PetReservation) {
                    if (p.PetId == r.PetId) {
                        inRes = true;
                    }
                }
                if (inRes == false) {
                    PetReservation.Add(new PetReservation() { PetId = p.PetId });
                }
            }
        }

        public string GetReservationStatusName(decimal statusCode) {
            string Name = "";

            switch (statusCode) {
                case 1:
                    Name = "pending";
                    break;

                case 2:
                    Name = "confirmed";
                    break;

                case 3:
                    Name = "active";
                    break;

                case 4:
                    Name = "cancelled";
                    break;

                case 5:
                    Name = "completed";
                    break;

                default:
                    Name = "No status";
                    break;

            }
            return Name;

        }

        [NotMapped]
        public double Subtotal { get; set; }

        public void AddToSubtotal(decimal price, double days) {
            this.Subtotal += (double)price * days;
        }

        public decimal AddPetDiscount(decimal rate, decimal discount) {
            return -(rate * discount);
        }

        public string DisplayPercentage(decimal num) {
            return num * 100 + "%";
        }

        public double CalculateTotalCost(double days, decimal serviceTotal) {
            return days * (double)serviceTotal;
        }

        public double GetReservationDays(DateTime start, DateTime end) {
            return (end - start).TotalDays;
        }

        public double CalculateReservationDiscount(decimal d) {
            return this.Subtotal * (double)d;
        }

        public double CalculateHST() {
            double temp = 0.0;
            temp = this.Subtotal * GST + this.Subtotal * QST;
            this.Subtotal += temp;
            return temp;
        }

        public void SetUpAllPetVaccinations() {
            foreach (var pr in this.PetReservation) {
                pr.Pet.AllPetVaccinations = null;
                PetVaccination[] allPetVaccinations = new PetVaccination[6];
                foreach (var pv in pr.Pet.PetVaccination) {
                    allPetVaccinations[pv.VaccinationId - 1] = pv;
                }
                pr.Pet.AllPetVaccinations = allPetVaccinations.ToArray();
            }
        }

        public async Task<bool> CheckPetVaccinations(Microsoft.AspNetCore.Http.IFormCollection form) {

            var vaccinationIds = form["VaccinationIds"];
            var vaccinationExpireDates = form["Vaccination"];

            this.SetUpAllPetVaccinations();

            bool isValid = true;

            foreach (var pr in this.PetReservation) {

                int i = 0;

                foreach (var pv in pr.Pet.AllPetVaccinations) {

                    if (form["Vaccination"][i] != "") {

                        DateTime expireDate = DateTime.Parse(form["Vaccination"][i]);

                        if (expireDate > pr.Reservation.EndDate) {

                            if (pv != null) {

                                if (pv.ExpiryDate != expireDate) {
                                    pv.ExpiryDate = expireDate;
                                }

                            } else {

                                PetVaccination newPv = new PetVaccination() {
                                    PetId = pr.Pet.PetId,
                                    ExpiryDate = DateTime.Parse(form["Vaccination"][i]),
                                    VaccinationId = int.Parse(form["VaccinationIds"][i]),
                                    VaccinationChecked = true
                                };

                                _context.Add(newPv);

                            }

                        } else {
                            isValid = false;
                        }
                    } else {

                        var petVaccinationToRemove = _context.PetVaccination.Where(v => v.VaccinationId == i + 1 && v.PetId == pr.PetId).FirstOrDefault();

                        if(petVaccinationToRemove != null) {
                            _context.PetVaccination.Remove(petVaccinationToRemove);
                        }

                        isValid = false;
                    }

                    i++;
                }
            }

            await _context.SaveChangesAsync();

            this.SetUpAllPetVaccinations();

            return isValid;
        }


        public void SetUpPetReservationRuns() {
            this.Status = 3;

            foreach (var petRes in this.PetReservation) {
                var run = _context.Run.Where(r => r.Status == 1 && r.Size == petRes.Pet.DogSize).FirstOrDefault();

                var allPetReservations = _context.PetReservation
                                            .Where(p => p.Reservation.StartDate <= petRes.Reservation.EndDate && p.Reservation.EndDate >= petRes.Reservation.StartDate && p.RunId == run.RunId)
                                            .Include(p => p.Reservation)
                                            .ToList();

                if (allPetReservations.Count == 0) {
                    petRes.RunId = run.RunId;
                    _context.Update(petRes);
                    _context.SaveChangesAsync();
                }
            }

            _context.Update(this);
            _context.SaveChangesAsync();
        }

        public async Task CreateReservation(IFormCollection form) {

            var medicationsName = form["MedicationName"];
            var medicationsEndDate = form["MedicationEndDate"];
            var medicationsDosage = form["MedicationDosage"];
            var medicationsSpecialInstruct = form["MedicationSpecialInstruct"];

            Reservation reservation = new Reservation() {
                StartDate = this.StartDate,
                EndDate = this.EndDate
            };
            _context.Add(reservation);
            await _context.SaveChangesAsync();

            int petResNumb = 0;
            foreach (var petRes in this.PetReservation) {
                string inResBoolName = petRes.PetId.ToString() + "-inResBool";
                var inResBool = form[inResBoolName];
                if (petRes.InReservation == true || inResBool == "true") {
                    PetReservation newPR = new PetReservation() {
                        ReservationId = reservation.ReservationId,
                        PetId = petRes.PetId
                    };
                    _context.Add(newPR);
                    await _context.SaveChangesAsync();

                    if (medicationsName.Any()) {
                        for (int i = 0; i < medicationsName.Count; i++) {
                            if (medicationsName[i] != "") {
                                Medication med = new Medication() {
                                    Name = medicationsName[i],
                                    Dosage = medicationsDosage[i],
                                    EndDate = DateTime.Parse(medicationsEndDate[i]),
                                    SpecialInstruct = medicationsSpecialInstruct[i],
                                    PetReservationId = newPR.PetReservationId
                                };
                                _context.Add(med);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    petResNumb++;

                    //Get Service list to allow to find their id's
                    var services = await(from s in _context.Service
                                         select s).ToListAsync();

                    // Boarding
                    var boardingService = services.Where(x => x.ServiceDescription.ToLower() == "boarding").FirstOrDefault();

                    PetReservationService boarding = new PetReservationService() {
                        PetReservationId = newPR.PetReservationId,
                        ServiceId = boardingService.ServiceId
                    };
                    _context.Add(boarding);
                    await _context.SaveChangesAsync();

                    // Daily Walks
                    string walkBoolName = petRes.PetId.ToString() + "-walkBool";
                    var walkBool = form[walkBoolName];

                    if (petRes.Walk == true || walkBool == "true") {
                        var walkService = services.Where(x => x.ServiceDescription.ToLower() == "walk").FirstOrDefault();

                        PetReservationService walk = new PetReservationService() {
                            PetReservationId = newPR.PetReservationId,
                            ServiceId = walkService.ServiceId
                        };
                        _context.Add(walk);
                        await _context.SaveChangesAsync();
                    }

                    //Daily Playtime
                    string playtimeBoolName = petRes.PetId.ToString() + "-playtimeBool";
                    var playtimeBool = form[playtimeBoolName];

                    if (petRes.Playtime == true || playtimeBool == "true") {
                        var playtimeService = services.Where(x => x.ServiceDescription.ToLower() == "playtime").FirstOrDefault();

                        PetReservationService playtime = new PetReservationService() {
                            PetReservationId = newPR.PetReservationId,
                            ServiceId = playtimeService.ServiceId
                        };
                        _context.Add(playtime);
                        await _context.SaveChangesAsync();
                    }
                }

                // Discount: Three or more pets 7% off total 
                if (petResNumb >= 3) {
                    ReservationDiscount multPetDis = new ReservationDiscount() {
                        DiscountId = 2,
                        ReservationId = reservation.ReservationId
                    };
                    _context.Add(multPetDis);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveReservation() {
            //Removing PetReservations tables to be able to update Reservation
            foreach (var petRes in this.PetReservation) {

                var medication = _context.Medication.Where(x => x.PetReservationId == petRes.PetReservationId).ToList();
                if (medication.Count >= 1) {
                    foreach (Medication med in medication) {
                        _context.Medication.Remove(med);
                    }
                }

                var petResDiscount = _context.PetReservationDiscount.Where(x => x.PetReservationId == petRes.PetReservationId).ToList();
                if (petResDiscount.Count >= 1) {
                    foreach (PetReservationDiscount prd in petResDiscount) {
                        _context.PetReservationDiscount.Remove(prd);
                    }
                }

                var petResServ = _context.PetReservationService.Where(x => x.PetReservationId == petRes.PetReservationId).ToList();
                if (petResServ.Count >= 1) {
                    foreach (PetReservationService prs in petResServ) {
                        _context.PetReservationService.Remove(prs);
                    }
                }

                var kennelLog = _context.KennelLog.Where(x => x.PetReservationId == petRes.PetReservationId).FirstOrDefault();
                if (kennelLog != null) {
                    _context.KennelLog.Remove(kennelLog);
                }

                _context.PetReservation.Remove(petRes);

                await _context.SaveChangesAsync();
            }

            var discount = _context.ReservationDiscount.Where(x => x.ReservationId == this.ReservationId).ToList();
            if (discount != null) {
                foreach (ReservationDiscount rm in discount) {
                    _context.ReservationDiscount.Remove(rm);
                }
            }

            _context.Remove(this);
            await _context.SaveChangesAsync();
        }
    }
}