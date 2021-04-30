using HVK_Queen.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HVK_Queen.Controllers {
    public class HomeController : Controller {
        private readonly HVK_QueenContext _context;

        public HomeController(HVK_QueenContext context) {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login login) {
            if (!ModelState.IsValid) {
                return View(login);
            } else {
                try {
                    var user = (from owner in _context.Owner
                                where owner.Phone == login.EmailOrPhone
                                     || owner.Email == login.EmailOrPhone
                                     && owner.Password == login.Password
                                select owner).FirstOrDefault();
                    if (user == null) {
                        var username = (from owner in _context.Owner
                                        where owner.Phone == login.EmailOrPhone
                                             || owner.Email == login.EmailOrPhone
                                        select owner).FirstOrDefault();
                        if (username == null) {
                            ViewBag.ErrorMess = "This Username is invalid";
                        } else {
                            ViewBag.ErrorMess = "This password is invalid";
                        }
                        return View(login);
                    } else {
                        if (login.CheckIfClerk(login.EmailOrPhone)) {
                            return RedirectToAction("Index", "Clerk");
                        } else {
                            HttpContext.Session.SetString("OwnerName", user.FirstName + " " + user.LastName);
                            HttpContext.Session.SetInt32("OwnerId", user.OwnerId);
                            return RedirectToAction("Index", "Owner");
                        }
                    }
                } catch {
                    return View(login);
                }

            }
        }
    }
}
