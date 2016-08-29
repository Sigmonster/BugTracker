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

        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            //UserRoles Helper
            var userRolesHelper = new UserRolesHelper(db);

            //Get Users
            var allSubmitters = userRolesHelper.GetAllUsersInRole("Submitter").OrderBy(u => u.DisplayName).ToList();
            var allDevelopers = userRolesHelper.GetAllUsersInRole("Developer").OrderBy(u => u.DisplayName).ToList();
            var allAdmins = userRolesHelper.GetAllUsersInRole("Admin").OrderBy(u => u.DisplayName).ToList();
            var allProjectManagers = userRolesHelper.GetAllUsersInRole("Project Manager").OrderBy(u => u.DisplayName).ToList();
            var allUsers = db.Users.OrderBy(u => u.DisplayName).ToList();

            //Get Tickets
            var allTickets = db.TicketPosts.OrderByDescending(t => t.Created).ToList();
            var criticalTickets = allTickets.Where(t => t.TicketPriority.Name == "Critical");
            var majorTickets = allTickets.Where(t => t.TicketPriority.Name == "Major");
            var minorTickets = allTickets.Where(t => t.TicketPriority.Name == "Minor");
            var trivialTickets = allTickets.Where(t => t.TicketPriority.Name == "Trivial");
            var blockerTickets = allTickets.Where(t => t.TicketPriority.Name == "Blocker");

            //### Home Tab Start ###
            ViewBag.TotalTickets = allTickets.Count();
            ViewBag.TotalProjects = db.Projects.Count();
            ViewBag.TotalUsers = allUsers.Count();
            ViewBag.TotalRoles = db.Roles.Count();
            ViewData["allProjectManagers"] = allProjectManagers;
            ViewData["allAdmins"] = allAdmins;
            //### Home Tab End ###

            //### User Tab Start ###
            //Instantiate View Models
            var AdminUsersVMList = new List<AdminUsersVM>();
            var AllUsersVM = new AdminUsersVM();
            var AllAdminsVM = new AdminUsersVM();
            var AllPMsVM = new AdminUsersVM();
            var AllDevsVM = new AdminUsersVM();
            var AllSubsVM = new AdminUsersVM();

            //Build View Model for All users, Admins, PMs, Developers, Submitters.
            AllUsersVM.Title = "All Users";
            AllUsersVM.Users = allUsers;
            AdminUsersVMList.Add(AllUsersVM);
            AllAdminsVM.Title = "All Admins";
            AllAdminsVM.Users = allAdmins;
            AdminUsersVMList.Add(AllAdminsVM);
            AllPMsVM.Title = "All Project Managers";
            AllPMsVM.Users = allProjectManagers;
            AdminUsersVMList.Add(AllPMsVM);
            AllDevsVM.Title = "All Developers";
            AllDevsVM.Users = allDevelopers;
            AdminUsersVMList.Add(AllDevsVM);
            AllSubsVM.Title = "All Submitters";
            AllSubsVM.Users = allSubmitters;
            AdminUsersVMList.Add(AllSubsVM);
            //Admin Users Tab Data
            ViewBag.Admins = allAdmins.Count();
            ViewBag.ProjectManagers = allProjectManagers.Count();
            ViewBag.Developers = allDevelopers.Count();
            ViewBag.Submitters = allSubmitters.Count();
            ViewData["AdminUsersVMList"] = (List<AdminUsersVM>)AdminUsersVMList;
            //### User Tab End ###

            //### Tickets Tab Start ###
            var AdminTicketsVMList = new List<AdminTicketsVM>();
            AdminTicketsVMList.Add(new AdminTicketsVM { Title = "All Tickets: " + allTickets.Count(), Tickets = allTickets });
            AdminTicketsVMList.Add(new AdminTicketsVM { Title = "Critial Tickets: " + criticalTickets.Count(), Tickets = criticalTickets.ToList() });
            AdminTicketsVMList.Add(new AdminTicketsVM { Title = "Major Tickets: " + majorTickets.Count(), Tickets = majorTickets.ToList() });
            AdminTicketsVMList.Add(new AdminTicketsVM { Title = "Minor Tickets: " + minorTickets.Count(), Tickets = minorTickets.ToList() });
            AdminTicketsVMList.Add(new AdminTicketsVM { Title = "Trivial Tickets: " + trivialTickets.Count(), Tickets = trivialTickets.ToList() });
            AdminTicketsVMList.Add(new AdminTicketsVM { Title = "Blocker Tickets: " + blockerTickets.Count(), Tickets = blockerTickets.ToList() });

            ViewData["AdminTicketsVMList"] = (List<AdminTicketsVM>)AdminTicketsVMList;
            ViewBag.AllTickets = allTickets.Count();
            ViewBag.OpenTickets = allTickets.Where(t => t.TicketStatus.Name == "Open").Count();
            ViewBag.InProgress = allTickets.Where(t => t.TicketStatus.Name == "InProgress").Count();
            ViewBag.Pending = allTickets.Where(t => t.TicketStatus.Name == "Pending").Count();
            ViewBag.Resolved = allTickets.Where(t => t.TicketStatus.Name == "Resolved").Count();
            //### Tickets Tab Start ###

            //### Admin Settings Start ###
            ViewData["Roles"] = (List<IdentityRole>)db.Roles.ToList();
            ViewData["Statuses"] = db.TicketStatuses.ToList();
            ViewData["Types"] = db.TicketTypes.ToList();
            ViewData["Priorities"] = db.TicketPriorities.ToList();
            //### Admin Settings End ###

            return View();
        }


        //
        //######################Start Edit Users Section########################
        //ALL User List Section
        // GET: Admin/EditUsers
        [Authorize(Roles = "Admin")]
        public ActionResult EditUsers()
        {
            var users = db.Users.ToList();

            ViewData["AllUsers"] = users;
            return View();
        }

        //
        //Get Edit User Roles
        //Get Edit User Roles Section
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModelUserView = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var selected = helper.ListUserRoles(id);
            AdminModelUserView.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModelUserView.Id = user.Id;
            AdminModelUserView.Name = user.DisplayName;
            AdminModelUserView.User = user;
            return View(AdminModelUserView);
        }
        //
        //POST Edit User Roles
        //POST Edit User Roles Section
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(/*[Bind(Include = "User, Roles, SelectedRoles, Id, Name")]*/ AdminUserViewModel AdminUserViewModel)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            string userId = AdminUserViewModel.Id;

            if (!User.IsInRole("DemoAcc"))
            {
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
            }
            return RedirectToAction("EditUserRoles", "Admin", userId);
        }
        //End POST Edit User Roles
        //
        //######################Start Edit Users Section########################

        //Start Admin Projects Start Section
        //ALL Projects List Section
        // GET: Admin/Projects
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Projects()
        {
            var allProjects = db.Projects.ToList().OrderBy(m => m.Name);
            var activeProjects = allProjects.Where(p => p.Active == true);
            var inactiveProjects = allProjects.Where(p => p.Active == false);

            ViewData["ActiveProjects"] = activeProjects;
            ViewData["InactiveProjects"] = inactiveProjects;

            return View();
        }
        //
        // GET:Admin/ProjectsEdit
        [Authorize( Roles = "Admin, Project Manager")]
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
        [Authorize(Roles = "Admin, Project Manager")]
        public async Task<ActionResult> ProjectEdit([Bind(Include = "Id,Name,Active")] Projects projects)
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());//User that submitted form
            var currentProject = db.Projects.Find(projects.Id);
            var allowed = false;

            if (User.IsInRole("Admin"))
            {
                allowed = true;
            }
            else if (User.IsInRole("Project Manager") && currentProject.Users.Contains(currentUser))
            {
                allowed = true;
            }
            if (User.IsInRole("DemoAcc"))
            {
                allowed = false;
            }


            if (ModelState.IsValid && allowed == true)
            {
                currentProject.Active = projects.Active;
                currentProject.Name = projects.Name;

                if (currentProject.Active == false)
                {
                    var usersArr = currentProject.Users.ToArray();
                    for (var i=0; i < usersArr.Length; i++)
                    {
                        projects.Users.Remove(usersArr[i]);
                    }
                }

                db.Entry(currentProject).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewData["MessageSuccess"] = "Update Sucessful!";
                return View(projects);
            }
            else if (!ModelState.IsValid)
            {
                ViewData["MessageFail"] = "Update Failed!";
            }
            
            return View(projects);
        }

        // GET: Admin/ProjectCreate
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ProjectCreate()
        {
            return View();
        }

        // POST: Admin/ProjectCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public async Task<ActionResult> ProjectCreate([Bind(Include = "Id,Name")] Projects projects)
        {
            if (!User.IsInRole("DemoAcc"))
            {
                if (ModelState.IsValid)
                {
                    db.Projects.Add(projects);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Projects", "Admin");
                }
            }

            return View(projects);
        }

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