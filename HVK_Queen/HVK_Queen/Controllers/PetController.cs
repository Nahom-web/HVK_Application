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

    public class PetController : Controller {
        private readonly HVK_QueenContext _context;

        public PetController(HVK_QueenContext context) {
            _context = context;
        }

        // GET: Pet/Create
        [HttpGet]
        public IActionResult Create() {
            var ownerId = HttpContext.Session.GetInt32("OwnerId").Value;
            ViewBag.OwnerId = ownerId;
            ViewData["VaccinationId"] = new SelectList(_context.Vaccination, "VaccinationId", "Name", ownerId);
            return View();
        }

        // POST: Pet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetId,Name,Gender,Fixed,Breed,Birthdate,OwnerId,DogSize,Climber,Barker")] Pet pet, IFormCollection form) {
            var ownerId = HttpContext.Session.GetInt32("OwnerId").Value;

            pet.Gender = pet.getGender(form["Gender"]);

            pet.DogSize = pet.getDogSize(form["DogSize"]);

            ViewData["VaccinationId"] = new SelectList(_context.Vaccination, "VaccinationId", "Name", ownerId);

            if (ModelState.IsValid) {
                pet.OwnerId = ownerId;
                pet.Owner = _context.Owner.Where(p => p.OwnerId == ownerId).FirstOrDefault();
                _context.Add(pet);
                await _context.SaveChangesAsync();

                var notes = form["SpecialNotes.Notes"];
                if (!String.IsNullOrEmpty(notes)) {
                    SpecialNotes specialNotes = new SpecialNotes() {
                        Notes = notes,
                        PetId = pet.PetId
                    };
                    _context.Add(specialNotes);
                }

                var vaccinations = form["PetVaccination"];
                var vaccinationName = form["VaccinationName"];
                for (var i = 0; i < vaccinations.Count(); i++) {
                    if (!String.IsNullOrEmpty(vaccinations[i])) {
                        PetVaccination petVaccination = new PetVaccination() {
                            ExpiryDate = Convert.ToDateTime(vaccinations[i]),
                            PetId = pet.PetId,
                            VaccinationId = i + 1
                        };
                        _context.Add(petVaccination);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Owner", new { id = ownerId });
            }
            return View(pet);
        }

        // GET: Pet/Details/10
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var pet = await _context.Pet
                .Include(p => p.Owner)
                .Include(s => s.SpecialNotes)
                .Include(pv => pv.PetVaccination)
                .ThenInclude(v => v.Vaccination)
                .FirstOrDefaultAsync(m => m.PetId == id);

            if (pet == null) {
                return NotFound();
            }
            return View(pet);
        }

        // GET: Pet/Edit/10
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var pet = await _context.Pet
                .Include(p => p.Owner)
                .Include(s => s.SpecialNotes)
                .Include(pv => pv.PetVaccination)
                .ThenInclude(v => v.Vaccination)
                .FirstOrDefaultAsync(m => m.PetId == id);

            if (pet == null) {
                return NotFound();
            } else {
                ViewBag.Gender = pet.getGenderFull(pet.Gender);
                ViewBag.DogSize = pet.getDogSizefull(pet.DogSize);
            }
            ViewData["VaccinationId"] = _context.Vaccination.ToList();

            ViewBag.petVaccinations = new PetVaccination[6];
            foreach (var petvaccination in pet.PetVaccination) {
                ViewBag.petVaccinations[petvaccination.VaccinationId - 1] = petvaccination;
            }

            return View(pet);
        }

        // POST: Pet/Edit/10
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pet pet, IFormCollection form) {
            if (id != pet.PetId) {
                return NotFound();
            }
            pet.Gender = pet.getGender(form["Gender"]);
            pet.DogSize = pet.getDogSize(form["DogSize"]);

            if (ModelState.IsValid) {
                try {
                    var specialnotes = await _context.SpecialNotes
                                            .Where(s => s.PetId == id)
                                            .FirstOrDefaultAsync();
                    var notes = pet.SpecialNotes.Notes;

                    if (specialnotes != null) {
                        if (notes != null) {
                            specialnotes.Notes = notes;
                            _context.Update(specialnotes);
                            await _context.SaveChangesAsync();
                            pet.SpecialNotes = specialnotes;
                        } else {
                            _context.SpecialNotes.Remove(specialnotes);
                            pet.SpecialNotes = null;

                        }
                    } else {
                        if (notes != null) {
                            SpecialNotes specialNotes = new SpecialNotes() {
                                Notes = notes,
                                PetId = pet.PetId
                            };
                            _context.Add(specialNotes);
                            await _context.SaveChangesAsync();
                            pet.SpecialNotes = specialNotes;
                        } else {
                            pet.SpecialNotes = null;
                        }
                    }

                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                    var petvaccinations = await _context.PetVaccination
                                           .Where(p => p.PetId == id)
                                           .Include(v => v.Vaccination)
                                           .ToListAsync();

                    var vaccinations = form["PetVaccination"];
                    var vaccinationName = form["VaccinationName"];
                    ViewBag.petVaccinations = new PetVaccination[6];
                    foreach (var petvaccination in pet.PetVaccination) {
                        ViewBag.petVaccinations[petvaccination.VaccinationId - 1] = petvaccination;
                    }
                    for (var i = 0; i < vaccinations.Count(); i++) {
                        if (!String.IsNullOrEmpty(vaccinations[i])) {
                            if (ViewBag.petVaccinations[i] != null) {
                                ViewBag.petVaccinations[i].ExpiryDate = Convert.ToDateTime(vaccinations[i]);
                                _context.Update(ViewBag.petVaccinations[i]);
                            } else {
                                PetVaccination petVaccination = new PetVaccination() {
                                    ExpiryDate = Convert.ToDateTime(vaccinations[i]),
                                    PetId = pet.PetId,
                                    VaccinationId = i + 1
                                };
                                _context.Add(petVaccination);
                            }
                        }

                    }
                    await _context.SaveChangesAsync();

                } catch (DbUpdateConcurrencyException) {
                    if (!PetExists(pet.PetId)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Owner", new { id = pet.OwnerId });
            }
            ViewData["VaccinationId"] = new SelectList(_context.Vaccination, "VaccinationId", "Name");
            return View(pet);
        }

        private string getGender(Microsoft.Extensions.Primitives.StringValues valueGender) {
            throw new NotImplementedException();
        }


        // GET: Pet/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var pet = await _context.Pet
                .Include(p => p.Owner)
                .Include(s => s.SpecialNotes)
                .Include(pv => pv.PetVaccination)
                .ThenInclude(v => v.Vaccination)
                .FirstOrDefaultAsync(m => m.PetId == id);

            if (pet == null) {
                return NotFound();
            } else {
                ViewBag.Gender = pet.getGenderFull(pet.Gender);
                ViewBag.DogSize = pet.getDogSizefull(pet.DogSize);
            }

            return View(pet);
        }

        // POST: Pet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var pet = await _context.Pet.FindAsync(id);
            var notes = await _context.SpecialNotes
                        .Where(n => n.PetId == id)
                        .FirstOrDefaultAsync();
            var petvaccination = await _context.PetVaccination
                        .Where(n => n.PetId == id)
                        .ToListAsync();
            if (notes != null) {
                _context.SpecialNotes.Remove(notes);
            }
            foreach (var vaccination in petvaccination) {
                _context.Remove(vaccination);
            }
            _context.Pet.Remove(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Owner", new { id = pet.OwnerId });
        }

        private bool PetExists(int id) {
            return _context.Pet.Any(e => e.PetId == id);
        }
    }
}
