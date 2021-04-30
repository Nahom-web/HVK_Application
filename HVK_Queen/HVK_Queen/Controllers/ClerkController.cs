using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HVK_Queen.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace HVK_Queen.Controllers {

    [Route("Clerk")]

    public class ClerkController : Controller {

        private readonly HVK_QueenContext _context;

        public ClerkController(HVK_QueenContext context) {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() {

            var reservations = _context.Reservation
                                   .Include(pr => pr.PetReservation)
                                   .ThenInclude(pet => pet.Pet)
                                   .ThenInclude(o => o.Owner)
                                   .ThenInclude(pet2 => pet2.Pet)
                                   .ThenInclude(petVac => petVac.PetVaccination)
                                   .OrderBy(sd => sd.StartDate);

            ViewData["StartingReservations"] = reservations
                                                .Where(s => s.StartDate == DateTime.Now.Date && s.Status != 3)
                                                .ToList();

            ViewData["EndingReservations"] = reservations
                                                .Where(s => s.EndDate == DateTime.Now.Date && s.Status != 5)
                                                .ToList();

            ViewData["OngoingReservations"] = reservations
                                                .Where(s => (s.StartDate < DateTime.Now.Date && s.EndDate > DateTime.Now.Date) || s.Status == 3 && s.EndDate != DateTime.Now.Date)
                                                .ToList();

            ViewData["FutureReservations"] = reservations
                                                .Where(s => s.StartDate > DateTime.Now.Date)
                                                .ToList();


            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormCollection form) {

            var ownerFromForm = form["NameOrPhoneNumber"];

            if (ownerFromForm == "") {
                return RedirectToAction("Index", "Clerk");
            }

            int FoundOwnerId = -1;

            var allOwners = (from o in _context.Owner
                             select o).ToList();

            foreach (var o in allOwners) {

                string fullName = o.FirstName + " " + o.LastName;

                // checking if they entered first name, last name, first name and last name, or phone number/cell phone number.

                if (ownerFromForm == fullName || ownerFromForm == o.FirstName || ownerFromForm == o.FirstName || ownerFromForm == o.Phone || ownerFromForm == o.CellPhone) {
                    FoundOwnerId = o.OwnerId;
                }
            }

            if (FoundOwnerId != -1) {
                return RedirectToAction("Index", "Owner", new { id = FoundOwnerId });
            } else {
                return RedirectToAction("Index", "Clerk");
            }
        }

        [HttpGet, Route("StartPetVisit/{id:int}")]
        public IActionResult StartPetVisit(int id) {

            var reservation = ReservationById(id);

            ViewData["AllVaccinations"] = _context.Vaccination.ToList();

            reservation.SetUpAllPetVaccinations();

            if (reservation != null) {
                if (reservation.GetReservationStatusName(reservation.Status) == "pending" || reservation.GetReservationStatusName(reservation.Status) == "confirmed") {
                    return View(reservation);
                } else {
                    return RedirectToAction("Index", "Clerk");
                }
            } else {
                return NotFound();
            }

        }

        [HttpPost, Route("StartPetVisit/{id:int}")]
        public async Task<IActionResult> StartPetVisit(Reservation res, IFormCollection form) {

            Reservation confirmedReservation = ReservationById(res.ReservationId);

            ViewData["AllVaccinations"] = _context.Vaccination.ToList();

            if (await confirmedReservation.CheckPetVaccinations(form)) {

                confirmedReservation.SetUpPetReservationRuns();

                return RedirectToAction("Contract", "Clerk", new { id = confirmedReservation.ReservationId });

            } else {

                ViewBag.VaccinationErrorMessage = "Please double check all vaccination expire dates.";

                return View(confirmedReservation);
            }

        }

        [Route("Contract/{id:int}")]

        public IActionResult Contract(int id) {

            var reservation = ReservationById(id);

            ViewBag.Days = (reservation.EndDate - reservation.StartDate).TotalDays;

            ViewData["ServiesList"] = from serv in _context.Service select serv.ServiceDescription;

            ViewBag.PartialViewMessage = "contract";

            return View(reservation);
        }

        [Route("EndPetVisit/{id:int}")]

        public IActionResult EndPetVisit(int id) {

            var reservation = ReservationById(id);

            if (reservation != null) {

                if (reservation.GetReservationStatusName(reservation.Status) == "active") {

                    return View(reservation);

                } else {

                    ViewBag.ErrorMessage = "Can't end an reservation that isn't active";

                    return RedirectToAction("Index", "Clerk");
                }
            } else {

                return NotFound();
            }

        }

        [Route("Invoice/{id:int}")]

        public async Task<IActionResult> Invoice(int id) {

            var reservation = ReservationById(id);

            if (reservation.Status == 3) {

                reservation.Status = 5;

                foreach (var pr in reservation.PetReservation) {
                    var run = await _context.Run.Where(r => r.RunId == pr.RunId).FirstOrDefaultAsync();

                    run.Status = 2;

                    _context.Update(run);

                    await _context.SaveChangesAsync();
                }

                _context.Update(reservation);

                await _context.SaveChangesAsync();

                ViewBag.Days = (reservation.EndDate - reservation.StartDate).TotalDays;

                return View(reservation);

            } else {
                ViewBag.ErrorMessage = "Can't get the invoice of an reservation that isn't completed";

                return RedirectToAction("Index", "Clerk");
            }

        }

        public Reservation ReservationById(int id) {

            // Getting all the relevent information for the reservation for the start pet visit, end pet visit, contract, invoice and owner home page.
            // I do not neccessarily need everything included for all the views but keeping it this way I can use the same query for all views.

            return _context.Reservation
                                  .Where(s => s.ReservationId == id)
                                  .Include(pr => pr.PetReservation)
                                  .ThenInclude(med => med.Medications)
                                  .Include(pr => pr.PetReservation)
                                  .ThenInclude(prs => prs.PetReservationService)
                                  .ThenInclude(s => s.Service)
                                  .ThenInclude(d => d.DailyRate)
                                  .Include(pr => pr.PetReservation)
                                  .ThenInclude(pet => pet.Pet)
                                  .ThenInclude(o => o.Owner)
                                  .Include(pr => pr.PetReservation)
                                  .ThenInclude(pet => pet.Pet)
                                  .ThenInclude(pv => pv.PetVaccination)
                                  .ThenInclude(v => v.Vaccination)
                                  .Include(pr => pr.PetReservation)
                                  .ThenInclude(pet => pet.Pet)
                                  .ThenInclude(sp => sp.SpecialNotes)
                                  .Include(pr => pr.PetReservation)
                                  .ThenInclude(petDisc => petDisc.PetReservationDiscount)
                                  .ThenInclude(disc => disc.Discount)
                                  .Include(petDic => petDic.ReservationDiscount)
                                  .ThenInclude(disc => disc.Discount)
                                  .Include(r => r.PetReservation)
                                  .ThenInclude(run => run.Run)
                                  .FirstOrDefault();
        }
    }
}