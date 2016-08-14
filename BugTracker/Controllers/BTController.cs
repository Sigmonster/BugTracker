using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.Collections;

namespace BugTracker.Controllers
{
    public class BTController : Controller
    {


        private ApplicationDbContext db = new ApplicationDbContext();

        //BT Projects Start Section
        //List the logged in User's Assigned Projects
        // GET: BT/MyProjects
        public ActionResult MyProjects()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var myProjects = user.Projects.ToList().OrderBy(m => m.Name);
            if (myProjects.Count() < 1 )//Checks if ther are any projects are assigned.
            {
                string errcode = "Access Denied, MyProjects, No Projects. User:" + user.UserName.ToString();
                return RedirectToAction("Err403", "BT", new { errcode = errcode });
            }
            
            return View(myProjects);
        }
        //
        //
        // GET: Admin/ProjectDetails/5
        public async Task<ActionResult> ProjectDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = await db.Projects.FindAsync(id);
            if (projects == null)
            {
                return HttpNotFound();
            }

            var pTickets= new DispTicketsVM();
            pTickets.TLTitle = "Tickets";
            pTickets.TLTitleDesc = "Showing Tickets from " + projects.Name.ToString();
            pTickets.TicketList = projects.Tickets.ToList();
            ViewData["ProjectTickets"] = projects.Tickets.ToList();
            
            return View(projects);
        }

        //
        //
        //GET : BT/EditProjects()
        public ActionResult EditProjectUsers(int id)
        {
            var EPBigVM = new EPBigVM();
            EPBigVM.Project = db.Projects.Find(id);
            EPBigVM.UsersAll = db.Users.ToList();
            EPBigVM.UsersInProject = EPBigVM.Project.Users.ToList();
            EPBigVM.UsersNotInProject = db.Users.ToList().Except(EPBigVM.UsersInProject).ToList();
            EPBigVM.SelectedRemoveUsers = EPBigVM.Project.Users.ToList();
            List<EPSelectedVM> list = new List<EPSelectedVM>();
            var notInProject = db.Users.ToList().Except(EPBigVM.UsersInProject).ToArray();

            for (var i = 0; i < notInProject.Length; i++)
            {
                list.Add(new EPSelectedVM() { UserDisplayName = notInProject[i].DisplayName, UserEmail = notInProject[i].Email, UserId = notInProject[i].Id, IsChecked = false});
            }
            List<EPRMSelectedVM> listRM = new List<EPRMSelectedVM>();
            var inProject = db.Projects.Find(id).Users.ToArray();
            for (var i = 0; i < inProject.Length; i++)
            {
                listRM.Add(new EPRMSelectedVM() { UserDisplayName = inProject[i].DisplayName, UserEmail = inProject[i].Email, UserId = inProject[i].Id, IsChecked = false });
            }
            //Not in Project
            EPBigVM.EPSelectedListVM = new EPSelectedListVM();
            EPBigVM.EPSelectedListVM.Users = list;
            //In Project
            EPBigVM.EPRMSelectedListVM = new EPRMSelectedListVM();
            EPBigVM.EPRMSelectedListVM.Users = listRM;

            //EPBigVM.EPSelectedVM = not needed?
            ViewBag.Title = ViewData["ProjectName"]= EPBigVM.Project.Name;
            ViewData["CurrentProject"] = id;
            return View(EPBigVM);
        }
        //Add users to project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectAddUser(EPSelectedListVM model, int Id)
        {
            if (!ModelState.IsValid) { 
                return RedirectToAction("EditProjectUsers", "BT", new { id = Id });}

            var currentProject = db.Projects.Find(Id);
            var selectedUsers = model.Users.Where(u => u.IsChecked.Equals(true)).ToList();//get's only users that were checked.
            var allUsers = db.Users;
            for (var i = 0; i < selectedUsers.Count(); i++)
            {
                var user = allUsers.Find(selectedUsers[i].UserId);//get current user.
                currentProject.Users.Add(user);
            }
            db.Entry(currentProject).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
        }
        //Remove users from Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectRMUser(EPRMSelectedListVM model, int Id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
            }

            var currentProject = db.Projects.Find(Id);
            var selectedUsers = model.Users.Where(u => u.IsChecked.Equals(true)).ToList();//get's only users that were checked.
            var allUsers = db.Users;
            for (var i = 0; i < selectedUsers.Count(); i++)
            {
                var user = allUsers.Find(selectedUsers[i].UserId);//get current user model.
                currentProject.Users.Remove(user);
            }
            db.Entry(currentProject).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
        }
        //
        //Error Pages Section
        //403 Forbidden
        public ActionResult Err403(string errcode)
        {
            ViewData["ErrorCode"] = errcode;
            return View();
        }

    }
}
