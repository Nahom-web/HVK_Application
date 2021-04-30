using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HVK_Queen.Models;
using Microsoft.AspNetCore.Http;

namespace HVK_Queen.Controllers {

    [Route("Owner")]

    public class OwnerController : Controller {

        private readonly HVK_QueenContext _context;

        public OwnerController(HVK_QueenContext context) {
            _context = context;
        }

        [Route("")]

        // Owner Home Page
        public async Task<IActionResult> Index() {

            int id = (int)HttpContext.Session.GetInt32("OwnerId");

            var owner = _context.Owner
                            .Where(o => o.OwnerId == id)
                            .Include(p => p.Pet)
                            .ThenInclude(petRes => petRes.PetReservation)
                            .ThenInclude(res => res.Reservation)
                            .ThenInclude(petRes2 => petRes2.PetReservation)
                            .Include(p2 => p2.Pet)
                            .ThenInclude(petVac => petVac.PetVaccination)
                            .FirstOrDefault();

            // Owners reservations to display on the home page

            ViewData["OwnersReservations"] = await _context.Reservation
                                                 .Include(pr => pr.PetReservation)
                                                 .ThenInclude(pet => pet.Pet)
                                                 .ThenInclude(o => o.Owner)
                                                 .Where(o => o.PetReservation.First().Pet.OwnerId == owner.OwnerId)
                                                 .ToListAsync();


            ViewBag.LogedInOwner = owner.FirstName + " " + owner.LastName;

            ViewBag.LogedInOwnerId = owner.OwnerId;

            if (owner == null) {
                return NotFound();
            }

            if (TempData["CannotDeleteAccountMessage"] != null) {
                ViewBag.CannotDeleteAccountMessage = TempData["CannotDeleteAccountMessage"].ToString();
            }

            return View(owner);
        }

        [Route("{id:int?}")]
        public async Task<IActionResult> IndexAsync(int? id) {

            if (id == null) {
                return NotFound();
            }

            var owner = _context.Owner
                            .Where(o => o.OwnerId == id)
                            .Include(p => p.Pet)
                            .ThenInclude(petRes => petRes.PetReservation)
                            .ThenInclude(res => res.Reservation)
                            .ThenInclude(petRes2 => petRes2.PetReservation)
                            .Include(p2 => p2.Pet)
                            .ThenInclude(petVac => petVac.PetVaccination)
                            .FirstOrDefault();

            ViewData["OwnersReservations"] = await _context.Reservation
                                     .Include(pr => pr.PetReservation)
                                     .ThenInclude(pet => pet.Pet)
                                     .ThenInclude(o => o.Owner)
                                     .Where(o => o.PetReservation.First().Pet.OwnerId == owner.OwnerId)
                                     .ToListAsync();

            ViewBag.LogedInOwner = owner.FirstName + " " + owner.LastName;

            ViewBag.LogedInOwnerId = owner.OwnerId;

            if (owner == null) {
                return NotFound();
            }

            if (TempData["CannotDeleteAccountMessage"] != null) {
                ViewBag.CannotDeleteAccountMessage = TempData["CannotDeleteAccountMessage"].ToString();
            }

            return View(owner);
        }

        [Route("Details/{id:int?}")]
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var owner = await _context.Owner
                .Include(o => o.Veterinarian)
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null) {
                return NotFound();
            }

            ViewBag.LogedInOwnerId = owner.OwnerId;

            return View(owner);
        }

        [HttpGet, Route("Create")]
        public IActionResult Create() {
            Owner owner = new Owner();

            ViewBag.LogedInOwnerId = owner.OwnerId;

            return View(owner);
        }

        [HttpPost, Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Owner owner) {
            if (ModelState.IsValid) {

                owner.UpdateOwnersInformation();

                owner.UpdateOwnersVetInformation();

                var allVets = await (_context.Veterinarian.Include(o => o.Owner).ToListAsync());

                // If the vet exists I find it with their phone number because it is unique. 

                if (owner.Veterinarian.Name != null) {

                    await owner.AddVetInformationAsync(allVets);

                } else {
                    owner.Veterinarian = null;
                }

                _context.Add(owner);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("OwnerName", owner.FirstName + " " + owner.LastName);
                HttpContext.Session.SetInt32("OwnerId", owner.OwnerId);

                return RedirectToAction("Index", "Owner");
            }
            ViewBag.LogedInOwnerId = owner.OwnerId;
            return View(owner);
        }

        [HttpGet, Route("Edit/{id:int?}")]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var owner = await _context.Owner
                .Include(o => o.Veterinarian)
                .FirstOrDefaultAsync(m => m.OwnerId == id);

            if (owner == null) {
                return NotFound();
            }

            ViewBag.LogedInOwnerId = owner.OwnerId;

            return View(owner);
        }

        [HttpPost, Route("Edit/{id:int?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Owner owner) {
            if (id != owner.OwnerId) {
                return NotFound();
            }
            if (ModelState.IsValid) {
                try {

                    owner.UpdateOwnersInformation();

                    owner.UpdateOwnersVetInformation();

                    var allVets = await (_context.Veterinarian.ToListAsync());

                    if (owner.Veterinarian.Name != null) {

                        await owner.AddVetInformationAsync(allVets);

                    } else {
                        owner.Veterinarian = null;
                    }

                    _context.Update(owner);

                    await _context.SaveChangesAsync();

                } catch (DbUpdateConcurrencyException) {
                    if (!OwnerExists(owner.OwnerId)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Owner");
            }
            ViewBag.LogedInOwnerId = owner.OwnerId;
            return View(owner);
        }

        [Route("Delete/{id:int?}")]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var owner = await _context.Owner
                .Include(o => o.Veterinarian)
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null) {
                return NotFound();
            }

            ViewBag.LogedInOwnerId = owner.OwnerId;

            return View(owner);
        }


        [HttpPost, Route("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Owner o) {

            var owner = await _context.Owner
                            .Where(ow => ow.OwnerId == o.OwnerId)
                            .Include(p => p.Pet)
                            .ThenInclude(pv => pv.PetVaccination)
                            .Include(p => p.Pet)
                            .ThenInclude(sp => sp.SpecialNotes)
                            .FirstOrDefaultAsync();

            // check if they have any active reservations

            var petReservations = await _context.PetReservation
                                   .Include(r => r.Reservation)
                                   .ThenInclude(pd => pd.ReservationDiscount)
                                   .Include(p => p.Pet)
                                   .ThenInclude(s => s.SpecialNotes)
                                   .ThenInclude(v => v.Pet.PetVaccination)
                                   .Include(p => p.Pet)
                                   .ThenInclude(o => o.Owner)
                                   .Include(m => m.Medications)
                                   .Include(prd => prd.PetReservationDiscount)
                                   .Include(prs => prs.PetReservationService)
                                   .Where(o => o.Pet.Owner.OwnerId == owner.OwnerId)
                                   .ToListAsync();

            var ongoingReservation = petReservations.Where(s => s.Reservation.StartDate < DateTime.Now.Date && s.Reservation.EndDate > DateTime.Now.Date).ToList();

            if (ongoingReservation != null && ongoingReservation.Count >= 1) {
                TempData["CannotDeleteAccountMessage"] = "Sorry, cannot delete account because you have active reservations";
                return RedirectToAction("Index", "Owner");
            }


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

            }

            owner.DeleteOwnersInformation(petReservations);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }



        private bool OwnerExists(int id) {
            return _context.Owner.Any(e => e.OwnerId == id);
        }
    }
}