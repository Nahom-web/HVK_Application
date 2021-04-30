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
    public class ReservationController : Controller {
        private readonly HVK_QueenContext _context;

        public ReservationController(HVK_QueenContext context) {
            _context = context;
        }

        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }
            int test = (int)id;

            var reservation = await (from petRes in _context.PetReservation
                                     .Include(prs => prs.PetReservationService)
                                     .ThenInclude(s => s.Service)
                                     join res in _context.Reservation on petRes.ReservationId equals res.ReservationId
                                     join pet in _context.Pet on petRes.PetId equals pet.PetId
                                     where res.ReservationId == id
                                     select new JoinForReservations { Reservation = res, PetReservation = petRes, Pet = pet })
                              .ToListAsync();

            if (reservation == null) {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservation/Create
        public IActionResult Create(PetReservation petRes) {
            int ownerId = HttpContext.Session.GetInt32("OwnerId").Value;

            ViewData["OwnerPets"] = _context.Pet.Where(x => x.OwnerId == ownerId);
            //Only works with the Owner Mike O'Phone (OwnerID = 2). Because the session failed to past the owner ID
            var reservation = new Reservation();

            return View(reservation);
        }

        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation postRes, IFormCollection form) { 
            if (ModelState.IsValid) {

                await postRes.CreateReservation(form);

                return RedirectToAction("Index", "Owner");
            }
            return View(postRes);
        }

        // GET: Reservation/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var reservation = await (from res in _context.Reservation
                                     .Include(x => x.PetReservation)
                                     .Include(x => x.PetReservation)
                                     .ThenInclude(x => x.PetReservationService)
                                     .ThenInclude(x => x.Service)
                                     .Include(x => x.PetReservation)
                                     .ThenInclude(x => x.PetReservationDiscount)
                                     .Include(x => x.PetReservation)
                                     .ThenInclude(x => x.Pet)
                                     where res.ReservationId == id  
                                   select res).FirstOrDefaultAsync();

            ViewData["OwnerPets"] = _context.Pet.Where(x => x.OwnerId == HttpContext.Session.GetInt32("OwnerId").Value);

            if (reservation == null) {
                return NotFound();
            }

            reservation.SetMissingPetRes(HttpContext.Session.GetInt32("OwnerId").Value);

            return View(reservation);
        }

        // POST: Reservation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation postRes, IFormCollection form) {
            if (id != postRes.ReservationId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    //Removing old Reservatione Tables
                    await postRes.RemoveReservation();

                    //Re adding all PetReservation tables
                    await postRes.CreateReservation(form);

                } catch (DbUpdateConcurrencyException) {
                    if (!ReservationExists(postRes.ReservationId)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Owner");
            }
            return View(postRes);
        }

        // GET: Reservation/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var reservation = await (from petRes in _context.PetReservation
                                     .Include(prs => prs.PetReservationService)
                                     .ThenInclude(s => s.Service)
                                     join res in _context.Reservation on petRes.ReservationId equals res.ReservationId
                                     join pet in _context.Pet on petRes.PetId equals pet.PetId
                                     where res.ReservationId == id
                                     select new JoinForReservations { Reservation = res, PetReservation = petRes, Pet = pet })
                              .ToListAsync();

            if (reservation == null) {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var reservation = await _context.Reservation.FindAsync(id);
            await reservation.RemoveReservation();

            return RedirectToAction("Index", "Owner");
        }

        private bool ReservationExists(int id) {
            return _context.Reservation.Any(e => e.ReservationId == id);
        }

        [HttpPost]
        public IActionResult CreateNewMedication(Reservation reservation, int petId) {

            int ownerId = HttpContext.Session.GetInt32("OwnerId").Value;
            ViewData["OwnerPets"] = _context.Pet.Where(x => x.OwnerId == ownerId);

            var petRes = reservation.PetReservation.Where(pr => pr.PetId == petId).FirstOrDefault();
            petRes.addMedication();

            reservation.PetReservation.Add(petRes);

            return View("Create", reservation);
        }
    }
}
