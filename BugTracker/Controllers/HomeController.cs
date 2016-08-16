using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {   
            //Uncomment to test if user roles are working.
            //var user = db.Users.Find(User.Identity.GetUserId());
            //var result = User.IsInRole("Registered User");
            //var result2 = User.IsInRole("Admin");
            
            if(User.IsInRole("Registered User"))
            {
                return RedirectToAction("FP403Error", "Error");
            }
            if (Request.IsAuthenticated)
            {
                return View();
            }

            return RedirectToAction("Login", "Account");
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}