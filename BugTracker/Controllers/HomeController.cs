using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {               
            if(User.IsInRole("Registered User"))
            {
                return RedirectToAction("FP403Error", "Error");
            }
            if (Request.IsAuthenticated)
            {
                var currentUser = db.Users.Find(User.Identity.GetUserId());
                var myNotifications = db.TicketNotifications.Where(x => x.UserID == currentUser.Id).ToList();
                var currentTime = DateTimeOffset.UtcNow;

                var myOwnedTicketsCount = db.TicketPosts.Where(x => x.OwnerUserID == currentUser.Id).Where(t=>t.TicketStatusID != 1).Count();
                var myAssignedTicketsCount = db.TicketPosts.Where(x => x.AssignedToUserID == currentUser.Id).Count();
                var myNotificationsCount = myNotifications.Count();
                var myNotificationsLast24Hrs = myNotifications.Where(x => x.Created > (currentTime.Subtract(new TimeSpan(24, 0, 0)))).Count();
                var myNotificationsLast72Hrs = myNotifications.Where(x => x.Created > (currentTime.Subtract(new TimeSpan(72, 0, 0)))).Count();
                var myNotificationsLast7Days = myNotifications.Where(x => x.Created > (currentTime.Subtract(new TimeSpan(168, 0, 0)))).Count();

                var myProjects = currentUser.Projects.OrderBy(m => m.Name).ToList();
                
                //UserRoles Helper
                var userRolesHelper = new UserRolesHelper(db);

                var myRoles = userRolesHelper.ListUserRoles(currentUser.Id).ToList();
                var allProjectManagers = userRolesHelper.GetAllUsersInRole("Project Manager").OrderBy(u => u.DisplayName).ToList();
                
                var MyHomeProjects = new MyHomeProjects();
                MyHomeProjects.allProjectManagers = new List<string>();
                MyHomeProjects.Projects = new List<Projects>();
                foreach (var item in myProjects)
                {
                    var projectManagersInProject = allProjectManagers.Where(u => u.Projects.Contains(item)).ToArray();
                    StringBuilder PMs = new StringBuilder();
                    for (int i = 0; i < projectManagersInProject.ToArray().Length; i++)
                    {

                        PMs.Append(projectManagersInProject[i].DisplayName);
                    if (i != (projectManagersInProject.ToArray().Length - 1))
                        {
                            PMs.Append(", ");
                        }
                    }
                    MyHomeProjects.allProjectManagers.Add(PMs.ToString());
                    MyHomeProjects.Projects.Add(item);
                }
                

                var MyHomeVM = new MyHomeVM();
                MyHomeVM.CurrentUser = currentUser;
                MyHomeVM.myOwnedTicketsCount = myOwnedTicketsCount;
                MyHomeVM.myAssignedTicketsCount = myAssignedTicketsCount;
                MyHomeVM.myNotificationsCount = myNotificationsCount;
                MyHomeVM.myNotificationsLast24Hrs = myNotificationsLast24Hrs;
                MyHomeVM.myNotificationsLast72Hrs = myNotificationsLast72Hrs;
                MyHomeVM.myNotificationsLast7Days = myNotificationsLast7Days;
                MyHomeVM.myProjects = myProjects;
                MyHomeVM.myRoles = myRoles;
                MyHomeVM.MyHomeProjects = MyHomeProjects;




            return View(MyHomeVM);
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