using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Security;

namespace BugTracker.App_Start
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Dashboard()
        {
            //var comboAdminModel = new AdminIndexViewModel();
            //comboAdminModel.AllProjects = db.Project.ToList();
            //comboAdminModel.AllTickets = db.Post.ToList();
            return View();
        }


        //
        //Start Edit Users Section
        //ALL User List Section
        // GET: Admin/EditUsers
        
        public ActionResult EditUsers()
        {
            //var users2= db.ApplicationUsers;
            //var users = db.Users.ToList();
            //AdminIndexViewModel AdminIndexViewModel = new AdminIndexViewModel();
            //AdminIndexViewModel.AllUsers = users;

            //return View(AdminIndexViewModel);

            var users = db.Users.ToList();
            //foreach (var role in users)
            //{
                
            //}

            ViewData["AllUsers"] = users;
            return View();
        }

        //
        //Get Edit User Roles
        //Get Edit User Roles Section
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModelUserView = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var selected = helper.ListUserRoles(id);
            AdminModelUserView.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModelUserView.Id = user.Id;
            AdminModelUserView.Name = user.DisplayName;
            return View(AdminModelUserView);
        }
        //
        //POST Edit User Roles
        //POST Edit User Roles Section
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserRoles(/*[Bind(Include = "User, Roles, SelectedRoles, Id, Name")]*/ AdminUserViewModel AdminUserViewModel)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            string userId = AdminUserViewModel.Id;
            if (ModelState.IsValid)
            {
                var selectedRoleList = AdminUserViewModel.SelectedRoles;
                string[] currentRoleList = helper.ListUserRoles(userId).ToArray();
                int counter = 0;//Counter for currentRoleList.
                //Spins through currentRoleList and removes roles that are nolonger selected
                foreach (var item in currentRoleList)
                {
                    bool present = false;
                    int counter2 = 0;//Counter for SelectedRolesList
                    while (present == false && counter2 < selectedRoleList.Length)
                    {
                        if (currentRoleList[counter] == selectedRoleList[counter2])
                        {
                            present = true;
                        }
                        counter2++;
                        if (present == false && counter2 == selectedRoleList.Length)
                        {
                            helper.RemoveUserFromRole(userId, currentRoleList[counter]);
                        }
                    }
                    counter++;
                }
                counter = 0;
                //Adds only roles that were selected.
                foreach (var item in selectedRoleList)
                {
                    string toBeAdded = selectedRoleList[counter];
                    if (!helper.IsUserInRole(userId, toBeAdded))
                    {
                        var result = helper.AddUserToRole(userId, toBeAdded);
                    }
                    counter += 1;
                }
                return RedirectToAction("EditUserRoles", "Admin", userId);
            }
            return RedirectToAction("EditUserRoles", "Admin", userId);
        }
        //End POST Edit User Roles
        //End Edit Users Section
        //

        //Start Admin Projects Start Section
        //ALL Projects List Section
        // GET: Admin/Projects
        public ActionResult Projects()
        {
           var projectlist = db.Projects.ToList().OrderBy(m => m.Name);
            return View(projectlist);
        }
        //
        // GET:Admin/ProjectsEdit
        public async Task<ActionResult> ProjectEdit(int? id)
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
            return View(projects);
        }

        // POST: Admin/ProjectsEdit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProjectEdit([Bind(Include = "Id,Name")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projects).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewData["Message"] = "Update Sucessful!";//Need to make this work, skipped because time constraint.
                return View(projects);
            }
            return View(projects);
        }

        // GET: Admin/ProjectCreate
        public ActionResult ProjectCreate()
        {
            return View();
        }

        // POST: Admin/ProjectCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProjectCreate([Bind(Include = "Id,Name")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(projects);
                await db.SaveChangesAsync();
                return RedirectToAction("Projects", "Admin");
            }

            return View(projects);
        }

        // GET: Admin/ProjectDelete/5
        public async Task<ActionResult> ProjectDelete(int? id)
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
            return View(projects);
        }

        // POST: Admin/ProjectDelete/5
        [HttpPost, ActionName("ProjectDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProjectDeleteConfirmed(int id)
        {
            Projects projects = await db.Projects.FindAsync(id);
            db.Projects.Remove(projects);
            await db.SaveChangesAsync();
            return RedirectToAction("Projects");
        }


        //
        //Get: Admin/ProjectAddUsers
        public ActionResult ProjectEditUsers(int? id)
        {

            return View(id);
        }

        //End Admin Project Section
        //Admin Projects Section
        //End Project Admin Projects Section
        //


        //Clean-UP!
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}